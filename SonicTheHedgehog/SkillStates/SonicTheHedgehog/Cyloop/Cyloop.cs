using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Survivors;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
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
        private int numIntersects;
        private Vector3 lastPosition;

        public EffectManagerHelper lineRendererObject;
        public LineRenderer lineRenderer;
        private JobHandle lineRendererPositionsJobHandle;
        private NativeArray<Vector3> lineRendererPositions;
        private float lineIntersectColorLerp;

        private JobHandle endJobHandle;
        private NativeArray<RaycastHit> targetsHit;
        public OverlapAttack overlapAttack;

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
            if (lineRendererObject.TryGetComponent<DestroyOnTimer>(out var trailDestroy))
            {
                trailDestroy.enabled = false;
            }

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
                            numIntersects--;
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
                        newIntersect[0] = -1;
                        numIntersects++;
                        Log.Message("current index:" + startingPointIndex);
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
            else if (endJobHandle.IsCompleted)
            {
                endJobHandle.Complete();
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
            if (lineRendererObject.TryGetComponent<DestroyOnTimer>(out var trailDestroy))
            {
                trailDestroy.enabled = true;
                if (lineRendererObject.TryGetComponent<AnimateShaderAlpha>(out var trailFade))
                {
                    trailFade.enabled = true;
                }
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
            temporaryOverlay.Destroy();
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
            if (numIntersects > 0)
            {
                ending = true;
                /*endJobHandle = new CyloopDivideIntersectedSectionsJob()
                {
                    cyloopLines = cyloopLines,
                }.Schedule(numValidPoints - 1, default);
                
                THE PLAN
                Create multiple convex shapes out of the concave shapes given from last job? 
                Rebuild the whole shape from scratch with easier-to-work-with points? Who says the points have to connect anymore? Just boxcast wider
                Use a stack to keep track of going one direction and going opposite to find opposite points? Might not work well if the same line is opposite of multiple points?
                Job goes through and gets info from the points, such as which point is across, direction vector, up vector, what width should boxcast be
                Make the shapes have even distance-between/distribution of points? Would need to be consistent if checking opposite end
                Go through all points and create boxcastcommand from it to the point on the opposite end of the shape
                Boxcast everything at once

                endJobHandle = new CyloopWriteBoxcastsJob()
                {
                
                }.Schedule(?, endJobHandle);
                endJobHandle = BoxcastCommand.ScheduleBatch(, , 10, endJobHandle);

                EffectManager.SpawnEffect(Modules.Assets.cyloopDebugHitboxVisual, new EffectData { origin = center, scale = halfExtents * 2f, rotation = orientation }, false);
                 */
                activatorSkillSlot.DeductStock(1);
                characterBody.OnSkillActivated(activatorSkillSlot);
            }
            else
            {
                this.outer.SetNextStateToMain();
            }
        }

        public virtual void PrepareAttack()
        {
            overlapAttack = new OverlapAttack();
            overlapAttack.attacker = gameObject;
            overlapAttack.inflictor = gameObject;
            overlapAttack.damage = StaticValues.cyloopDamageCoefficient * characterBody.damage;
            overlapAttack.damageType = DamageSource.Special;
            //overlapAttack.damageType.AddModdedDamageType(DamageTypes.cyloop)
            overlapAttack.forceVector = Vector3.up * 30f;
            overlapAttack.hitEffectPrefab = Modules.Assets.cyloopHitEffect;
            overlapAttack.isCrit = RollCrit(); // If I do multiple separate overlaps for different kinds of attacks, move this outside so it only happens once
            overlapAttack.teamIndex = characterBody.teamComponent.teamIndex;
        }
        // I want to use BoxcastCommands for the hit detection for performance reasons, but I also want OverlapAttack to handle networking for me
        // I take the RaycastHits from the BoxcastCommands and make my own OverlapAttack.OverlapInfo that I pass directly into the overlap attack
        private void RunOverlapAttack(List<RaycastHit> hits)
        {
            // Use a separate nativearray for just the raycasts that hit?
            List<OverlapAttack.OverlapInfo> overlapinfo = new List<OverlapAttack.OverlapInfo>();
            for (int i = 0; i < hits.Count; i++)
            {
                if (hits[i].collider.TryGetComponent<HurtBox>(out var hurtBox))
                {
                    overlapinfo.Add(new OverlapAttack.OverlapInfo { hurtBox = hurtBox, hitPosition = hits[i].point, pushDirection = Vector3.zero });
                }
            }
            overlapAttack.ProcessHits(overlapinfo);
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
        public int index; // these are kinda useless aside from invalid checking
        public float3 position;
        public float3 direction;
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
