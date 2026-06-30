using EntityStates;
using RoR2;
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
using static UnityEngine.ParticleSystem.PlaybackState;

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public class Cyloop : BaseSkillState, ISkillState
    {
        public const float minDuration = 0.5f;
        public const float timeBetweenUpdates = 0.25f;
        public const float minMoveSpeedPercent = 0.4f;

        public const int distanceFromPointsToIntersect = 2;
        public const float maxAngleDotForIntersection = 0.7f;

        public const float lineRendererIntersectColorFadeDuration = 0.4f;

        public virtual float cyloopLineIntersectWidth { get { return StaticValues.cyloopLineIntersectWidth; } }
        public virtual Color cyloopTrailColor { get{ return SonicTheHedgehogCharacter.sonicColor; } }
        public virtual Color cyloopTrailIntersectColor { get { return new Color(1f,0.3f,0.7f); } }

        private float updateTimer;

        public bool ending;
        private NativeArray<CyloopPoint> cyloopPoints;
        private NativeArray<CyloopLine> cyloopLines;
        private int numValidPoints;
        private int lastNumValidPoints;
        private int startingPointIndex = -1;

        private JobHandle intersectJobHandle;
        private NativeArray<bool> intersected;
        private Vector3 lastPosition;

        public EffectManagerHelper lineRendererObject;
        public LineRenderer lineRenderer;
        private JobHandle lineRendererPositionsJobHandle;
        private NativeArray<Vector3> lineRendererPositions;
        private float lineIntersectColorLerp;


        Run.FixedTimeStamp timeToLog;

        public override void OnEnter()
        {
            base.OnEnter();
            lineRendererObject = EffectManager.GetAndActivatePooledEffect(Modules.Assets.cyloopTrail, Vector3.zero, Quaternion.identity);
            lineRenderer = lineRendererObject.GetComponent<LineRenderer>();
            SetLineColor(cyloopTrailColor);

            cyloopPoints = new NativeArray<CyloopPoint>(StaticValues.cyloopMaxPoints, Allocator.Persistent);
            cyloopLines = new NativeArray<CyloopLine>(StaticValues.cyloopMaxPoints - 1, Allocator.Persistent);
            lineRendererPositions = new NativeArray<Vector3>(StaticValues.cyloopMaxPoints + 1, Allocator.Persistent);
            intersected = new NativeArray<bool>(1, Allocator.Persistent);
            timeToLog = Run.FixedTimeStamp.now + 1f;
            lastPosition = characterBody.corePosition;
            for (int i = 0; i < cyloopPoints.Length; i++)
            {
                cyloopPoints[i] = CyloopPoint.invalid;
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
                    cyloopPoints[startingPointIndex] = new CyloopPoint(characterBody.corePosition, characterMotor.velocity.normalized, startingPointIndex);
                    if (numValidPoints < StaticValues.cyloopMaxPoints) numValidPoints++;
                    if (cyloopPoints[(startingPointIndex - 1) % StaticValues.cyloopMaxPoints].IsValid())
                    {
                        cyloopLines[startingPointIndex % (StaticValues.cyloopMaxPoints - 1)] = new CyloopLine(cyloopPoints[startingPointIndex], cyloopPoints[(startingPointIndex - 1) % StaticValues.cyloopMaxPoints]);
                    }
                    if (numValidPoints < StaticValues.cyloopMaxPoints) Chat.AddMessage($"Cyloop points {numValidPoints}");
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

                    if (intersected[0])
                    {
                        lineIntersectColorLerp = 1f;
                        intersected[0] = false;
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
                        intersected = intersected
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
                lineRendererPositionsJobHandle = lineRendererPositionsJob.Schedule(numValidPoints + 1, 25, intersectJobHandle);
                lastNumValidPoints = numValidPoints;
                SetLineColor(Color.Lerp(cyloopTrailColor, cyloopTrailIntersectColor, lineIntersectColorLerp));
                if (lineIntersectColorLerp > 0)
                {
                    lineIntersectColorLerp -= Time.deltaTime * (1 / lineRendererIntersectColorFadeDuration);
                }
            }
        }

        public override void OnExit()
        {
            intersectJobHandle.Complete();
            lineRendererPositionsJobHandle.Complete();
            cyloopPoints.Dispose();
            cyloopLines.Dispose();
            lineRendererPositions.Dispose();
            intersected.Dispose();
            lineRendererObject.ReturnToPool();
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
            ending = true;
            foreach (var line in cyloopLines)
            {
                if (line.HasIntersect())
                {
                    Chat.AddMessage("Cylooped");
                    EffectManager.SimpleEffect(Modules.Assets.sonicBoomImpactEffect, line.lineIntersectPosition, Quaternion.identity, false);
                }
            }
            // make cyloop do shit here
            // if cyloop actually activated, reduce skill stock by 1
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
        public bool IsValid()
        {
            return point1.index != point2.index;
        }
        public bool HasIntersect()
        {
            return IsValid() && lineIntersectIndex != -1;
        }
    }
}
