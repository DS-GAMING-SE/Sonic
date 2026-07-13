using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static RoR2.CharacterSpeech.SolusHeartSpeechDriver;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public class Cyloop : BaseSkillState, ISkillState
    {
        public const float minDuration = 0.5f;
        public const float timeBetweenUpdates = 0.25f;
        public const float minMoveSpeedPercent = 0.4f;

        public const int distanceFromPointsToIntersect = 3;
        public const float maxAngleDotForIntersection = 0.7f;

        public const float lineRendererIntersectColorFadeDuration = 0.4f;

        public virtual float cyloopLineIntersectWidth { get { return StaticValues.cyloopLineIntersectWidth; } }
        public virtual float cyloopCollisionWidth { get { return StaticValues.cyloopCollisionWidth; } }
        public virtual Color cyloopTrailColor { get{ return SonicTheHedgehogCharacter.sonicColor2; } }
        public virtual Color cyloopTrailIntersectColor { get { return new Color(1f,0.3f,0.7f); } }
        public virtual float cyloopTrailSizeMultiplier { get { return 1f; } }

        public virtual Material temporaryOverlayMaterial { get { return Modules.Assets.cyloopOverlay; } }

        private float updateTimer;

        public bool ending;
        private NativeArray<CyloopPoint> cyloopPoints;
        private NativeArray<CyloopLine> cyloopLines;
        private int numValidPoints;
        private int lastNumValidPoints;
        private int startingPointIndex = -1;

        private JobHandle intersectJobHandle;
        private NativeArray<int> newIntersect;
        private int applyIntersectToNextLine;
        private Queue<int2> intersectIndices;
        private Vector3 lastPosition;

        public EffectManagerHelper lineRendererObject;
        public LineRenderer lineRenderer;
        private JobHandle lineRendererPositionsJobHandle;
        private NativeArray<Vector3> lineRendererPositions;
        private float lineIntersectColorLerp;

        private JobHandle endJobHandle;
        private NativeArray<JobHandle> endMeshCreationJobs;
        private Mesh.MeshDataArray endCollisionMesh;
        public OverlapAttack overlapAttack;
        private bool attacked;
        private int endNumSections;

        public EffectManagerHelper trailSpawningEffect;

        public TemporaryOverlayInstance temporaryOverlay;

        private SkillDef quickCyloopSkillDef;

        Run.FixedTimeStamp timeToLog;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_sonicthehedgehog_cyloop", gameObject);
            SpawnVFX();
            if (activatorSkillSlot.skillDef is SkillDefs.CyloopSkillDef cyloopSkillDef)
            {
                quickCyloopSkillDef = cyloopSkillDef.quickCyloopSkillDef;
                skillLocator.primary.SetSkillOverride(this, cyloopSkillDef.quickCyloopSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            activatorSkillSlot.onSkillChanged += OnSkillChanged;

            intersectIndices = new Queue<int2>();
            cyloopPoints = new NativeArray<CyloopPoint>(StaticValues.cyloopMaxPoints, Allocator.Persistent);
            cyloopLines = new NativeArray<CyloopLine>(StaticValues.cyloopMaxPoints - 1, Allocator.Persistent);
            lineRendererPositions = new NativeArray<Vector3>(StaticValues.cyloopMaxPoints + 1, Allocator.Persistent);
            newIntersect = new NativeArray<int>(1, Allocator.Persistent);
            newIntersect[0] = -1; // I wanted to use a job to save a value here and making a nativearray seemed like the way to do it
            applyIntersectToNextLine = -1;
            timeToLog = Run.FixedTimeStamp.now + 1f;
            lastPosition = characterBody.corePosition;
            for (int i = 0; i < cyloopPoints.Length; i++)
            {
                cyloopPoints[i] = CyloopPoint.invalid;
            }
        }

        private void SpawnVFX()
        {
            lineRendererObject = EffectManager.GetAndActivatePooledEffect(Modules.Assets.cyloopTrail, Vector3.zero, Quaternion.identity);
            lineRenderer = lineRendererObject.GetComponent<LineRenderer>();
            lineRenderer.widthCurve = AnimationCurve.Constant(0, 1, 1.5f * cyloopTrailSizeMultiplier);
            SetLineColor(cyloopTrailColor);

            trailSpawningEffect = EffectManager.GetAndActivatePooledEffect(Modules.Assets.cyloopTrailSpawningEffect, characterBody.coreTransform);
            trailSpawningEffect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            trailSpawningEffect.transform.localScale = Vector3.one * characterBody.radius * cyloopTrailSizeMultiplier;
            
            if (temporaryOverlayMaterial)
            {
                Transform modelTransform = GetModelTransform();
                if (modelTransform && modelTransform.TryGetComponent<CharacterModel>(out var model))
                {
                    temporaryOverlay = TemporaryOverlayManager.AddOverlay(model.gameObject);
                    temporaryOverlay.originalMaterial = temporaryOverlayMaterial;
                    temporaryOverlay.destroyComponentOnEnd = false;
                    temporaryOverlay.inspectorCharacterModel = model;
                    temporaryOverlay.Start(); // Apparently Start() isn't run if the overlay doesn't have animateShaderAlpha on so I gotta do this myself
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!ending)
            {
                updateTimer += Time.fixedDeltaTime;
                if (updateTimer >= timeBetweenUpdates)
                {
                    updateTimer %= timeBetweenUpdates;

                    startingPointIndex = (startingPointIndex + 1) % StaticValues.cyloopMaxPoints;
                    cyloopPoints[startingPointIndex] = new CyloopPoint(characterBody.corePosition, characterMotor.velocity, startingPointIndex);
                    if (numValidPoints < StaticValues.cyloopMaxPoints) numValidPoints++;
                    if (startingPointIndex > 0 && cyloopPoints[(startingPointIndex - 1) % StaticValues.cyloopMaxPoints].IsValid())
                    {
                        int newLineIndex = startingPointIndex - 1 % (StaticValues.cyloopMaxPoints - 1);
                        if (cyloopLines[newLineIndex].HasIntersect()) // Get rid of old intersects that expired
                        {
                            intersectIndices.Dequeue();
                            CyloopLine removingLineIntersectFromLine = cyloopLines[cyloopLines[newLineIndex].lineIntersectIndex];
                            removingLineIntersectFromLine.lineIntersectIndex = -1;
                            cyloopLines[cyloopLines[newLineIndex].lineIntersectIndex] = removingLineIntersectFromLine;
                        }
                        if (applyIntersectToNextLine != -1)
                        {
                            cyloopLines[newLineIndex] = new CyloopLine(cyloopPoints[startingPointIndex], cyloopPoints[(startingPointIndex - 1) % StaticValues.cyloopMaxPoints], applyIntersectToNextLine, cyloopLines[applyIntersectToNextLine].lineIntersectPosition);
                            applyIntersectToNextLine = -1;
                        }
                        else
                        {
                            cyloopLines[newLineIndex] = new CyloopLine(cyloopPoints[startingPointIndex], cyloopPoints[(startingPointIndex - 1) % StaticValues.cyloopMaxPoints]);
                        }
                        
                    }
                    //if (numValidPoints < StaticValues.cyloopMaxPoints) Chat.AddMessage($"Cyloop points {numValidPoints}");
                }
                if ((!inputBank.skill4.down || characterMotor.velocity.magnitude < characterBody.moveSpeed * minMoveSpeedPercent) && fixedAge >= minDuration)
                {
                    EndLoop();
                }
            }
        }
        public override void Update()
        {
            base.Update();
            if (!ending)
            {
                lineRendererPositionsJobHandle.Complete();

                if (lineRenderer && lastNumValidPoints > 0)
                {
                    lineRenderer.positionCount = lastNumValidPoints + 1;
                    lineRenderer.SetPositions(lineRendererPositions);

                    if (newIntersect[0] != -1)
                    {
                        EffectManager.SimpleEffect(Modules.Assets.sonicBoomImpactEffect, cyloopLines[newIntersect[0]].lineIntersectPosition, Quaternion.identity, false);
                        lineIntersectColorLerp = 1f;
                        applyIntersectToNextLine = newIntersect[0];
                        intersectIndices.Enqueue(new int2(newIntersect[0], (startingPointIndex-newIntersect[0]) % StaticValues.cyloopMaxPoints));
                        newIntersect[0] = -1;
                        //Log.Message("current index:" + startingPointIndex);
                    }
                }

                if (numValidPoints > distanceFromPointsToIntersect && isAuthority)
                {
                    intersectJobHandle.Complete();
                    CyloopIntersectionJob intersectJob = new CyloopIntersectionJob
                    {
                        characterPosition = characterBody.corePosition,
                        lastPosition = lastPosition,
                        lines = cyloopLines,
                        width = cyloopLineIntersectWidth,
                        startIndex = startingPointIndex,
                        ignoreFirstLines = distanceFromPointsToIntersect,
                        newIntersect = newIntersect
                    };
                    //intersectJob.Run(cyloopLines.Length);
                    intersectJobHandle = intersectJob.Schedule(cyloopLines.Length, default);
                    if (timeToLog.hasPassed)
                    {
                        timeToLog = Run.FixedTimeStamp.now + 1f;
                        // log something
                    }
                }
                lastPosition = characterBody.corePosition;

                CyloopLineRendererJob lineRendererPositionsJob = new CyloopLineRendererJob
                {
                    characterPosition = characterBody.corePosition,
                    points = cyloopPoints,
                    lerp = numValidPoints == StaticValues.cyloopMaxPoints ? (updateTimer / timeBetweenUpdates) : 0,
                    startIndex = startingPointIndex,
                    output = lineRendererPositions
                };
                lineRendererPositionsJobHandle = lineRendererPositionsJob.Schedule(numValidPoints + 1, 20, intersectJobHandle);
                lastNumValidPoints = numValidPoints;
                SetLineColor(Color.Lerp(cyloopTrailColor, cyloopTrailIntersectColor, lineIntersectColorLerp));
                if (lineIntersectColorLerp > 0)
                {
                    lineIntersectColorLerp -= Time.deltaTime * (1 / lineRendererIntersectColorFadeDuration);
                }
            }
            else if (endJobHandle.IsCompleted && isAuthority)
            {
                endJobHandle.Complete();
                attacked = true;
                Mesh[] meshes = new Mesh[endNumSections];
                for (int i = 0;i < meshes.Length;i++)
                {
                    Mesh.MeshData meshData = endCollisionMesh[i];
                    meshData.subMeshCount = 1;
                    meshData.SetSubMesh(0, new SubMeshDescriptor(0, meshData.GetIndexData<ushort>().Length));
                    meshes[i] = new Mesh();
                    meshes[i].name = "SonicCyloopCollisionMesh_" + i;
                }
                Mesh.ApplyAndDisposeWritableMeshData(endCollisionMesh, meshes, MeshUpdateFlags.DontValidateIndices);
                for (int i = 0; i < meshes.Length; i++)
                {
                    Physics.BakeMesh(meshes[i].GetInstanceID(), true);
                }
                Log.Message("Created " + meshes.Length + " CyloopCollider Mesh(es)");
                CyloopManager.GetPooledCyloopColliderController(this, meshes);

                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            activatorSkillSlot.onSkillChanged -= OnSkillChanged;
            intersectJobHandle.Complete();
            lineRendererPositionsJobHandle.Complete();
            endJobHandle.Complete();
            cyloopPoints.Dispose();
            cyloopLines.Dispose();
            lineRendererPositions.Dispose();
            newIntersect.Dispose();
            if (endMeshCreationJobs.IsCreated) endMeshCreationJobs.Dispose();
            if (ending && !attacked) endCollisionMesh.Dispose();
            if (lineRendererObject.TryGetComponent<AnimateShaderAlpha>(out var trailFade))
            {
                trailFade.enabled = true;
                trailFade.Restart();
            }
            else
            {
                lineRendererObject.ReturnToPool();
            }
            if (trailSpawningEffect.TryGetComponent<DisableParticleEmissionAndDestroyOnTimer>(out var trailSpawnDestroy))
            {
                trailSpawnDestroy.DisableParticlesStartTimer();
            }
            else
            {
                trailSpawningEffect.ReturnToPool();
            }
            if (temporaryOverlay != null) temporaryOverlay.Destroy();
            if (quickCyloopSkillDef)
            {
                skillLocator.primary.UnsetSkillOverride(this, quickCyloopSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            Util.PlaySound("Stop_sonicthehedgehog_cyloop", gameObject);
            base.OnExit();
        }

        private void SetLineColor(Color color)
        {
            if (lineRenderer)
            {
                var gradient = lineRenderer.GetColorGradientCopy();
                gradient.colorKeys = new GradientColorKey[] { new GradientColorKey(color, 0f) };
                lineRenderer.SetColorGradient(gradient);
            }
        }

        public void EndLoop()
        {
            if (quickCyloopSkillDef)
            {
                skillLocator.primary.UnsetSkillOverride(this, quickCyloopSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                quickCyloopSkillDef = null;
            }
            if (intersectIndices.Count > 0 && isAuthority)
            {
                ending = true;
                endNumSections = intersectIndices.Count; // Add concave points here
                // Figure out how many separate meshes to make using intersect sections and concave points

                endCollisionMesh = Mesh.AllocateWritableMeshData(endNumSections);
                endMeshCreationJobs = new NativeArray<JobHandle>(endNumSections * 2, Allocator.TempJob);
                for (int i = 0; i < endNumSections; i++)
                {
                    NativeArray<CyloopPoint> points = cyloopPoints.GetSubArray(intersectIndices.Peek().x, intersectIndices.Dequeue().y);
                    Mesh.MeshData cyloopCollisionMeshData = endCollisionMesh[i];

                    cyloopCollisionMeshData.SetVertexBufferParams(points.Length * 2, new VertexAttributeDescriptor(VertexAttribute.Position));
                    var pos = cyloopCollisionMeshData.GetVertexData<float3>();
                    endMeshCreationJobs[i * 2] = new CyloopMeshPointsJob()
                    {
                        points = points,
                        width = cyloopCollisionWidth,
                        output = pos
                    }.Schedule(pos.Length, 20);

                    cyloopCollisionMeshData.SetIndexBufferParams(pos.Length * 3, IndexFormat.UInt16);
                    var indexBuffer = cyloopCollisionMeshData.GetIndexData<ushort>();
                    // Trigger colliders must be convex so I only have to make the sides of the mesh and the top and bottom are auto-generated
                    endMeshCreationJobs[(i * 2) + 1] = new CyloopMeshTriangulateSidesJob()
                    {
                        length = pos.Length,
                        indexBuffer = indexBuffer
                    }.Schedule(pos.Length, 20);
                }
                endJobHandle = JobHandle.CombineDependencies(endMeshCreationJobs);
                 
                activatorSkillSlot.DeductStock(1);
                characterBody.OnSkillActivated(activatorSkillSlot);
            }
            else
            {
                this.outer.SetNextStateToMain();
            }
        }

        public virtual void PrepareAttack(ref OverlapAttack overlapAttack)
        {
            overlapAttack.Reset();
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.damage = StaticValues.cyloopDamageCoefficient * characterBody.damage;
            overlapAttack.damageType = DamageSource.Special;
            overlapAttack.damageType.AddModdedDamageType(DamageTypes.cyloop);
            overlapAttack.forceVector = Vector3.up * 33f;
            overlapAttack.physForceFlags |= PhysForceFlags.massIsOne | PhysForceFlags.resetVelocity | PhysForceFlags.respectKnockbackImmuneFlag;
            overlapAttack.hitEffectPrefab = Modules.Assets.cyloopHitEffect;
            overlapAttack.isCrit = RollCrit(); // If I do multiple separate overlaps for different kinds of attacks, move this outside so it only happens once
            overlapAttack.teamIndex = characterBody.teamComponent.teamIndex;
        }

        private void OnSkillChanged(GenericSkill skill)
        {
            this.outer.SetNextStateToMain();
        }
    }
    public struct CyloopPoint
    {
        public static readonly CyloopPoint invalid = new CyloopPoint(float3.zero, float3.zero, -1);
        public CyloopPoint(Vector3 position, Vector3 direction, int index = 0)
        {
            this.position = position;
            this.direction = direction;
            this.index = index;
        }
        public float3 position;
        public float3 direction;
        public int index; // these are kinda useless aside from invalid checking
        public bool IsValid()
        {
            return index != -1;
        }
    }
    public struct CyloopLine
    {
        public CyloopPoint point1;
        public CyloopPoint point2;
        public float3 lineDirection;
        public float3 lineCenterPosition;
        public int lineIntersectIndex;
        public float3 lineIntersectPosition;
        public CyloopLine(CyloopPoint point1, CyloopPoint point2)
        {
            this.point1 = point1;
            this.point2 = point2;
            lineDirection = point2.position - point1.position;
            lineCenterPosition = (point1.position + point2.position) / 2;
            lineIntersectIndex = -1;
            lineIntersectPosition = new float3(0, 0, 0);
        }
        public CyloopLine(CyloopPoint point1, CyloopPoint point2, int intersectIndex, float3 intersectPosition)
        {
            this.point1 = point1;
            this.point2 = point2;
            lineDirection = point2.position - point1.position;
            lineCenterPosition = (point1.position + point2.position) / 2;
            lineIntersectIndex = intersectIndex;
            lineIntersectPosition = intersectPosition;
        }
        public bool IsValid()
        {
            return point1.index != point2.index && point1.IsValid() && point2.IsValid();
        }
        public bool HasIntersect()
        {
            return IsValid() && lineIntersectIndex != -1;
        }
    }
}
