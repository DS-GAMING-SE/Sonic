using SonicTheHedgehog.SkillStates.Cyloop;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public struct CyloopDivideIntersectedSectionsJob : IJobFor
    {
        [ReadOnly]
        public NativeArray<CyloopLine> cyloopLines;
        // We're keeping track of how many intersects there are already, so we know how big the lists need to be
        [WriteOnly]
        public NativeList<CyloopPoint> outputIntersectPoints;
        [WriteOnly]
        public NativeList<int2> outputIntersectStartEndIndices;
        // Only using cyloopLines point2 here because point2 and the next line's point1 are the same (except for when it reaches end, does this cover that?)
        public void Execute(int i)
        {
            if (cyloopLines[i].HasIntersect())
            {
                outputIntersectPoints.AddNoResize(new CyloopPoint(cyloopLines[i].lineIntersectPosition, cyloopLines[i].lineDirection));
                outputIntersectStartEndIndices.AddNoResize(new int2(i, cyloopLines[i].lineIntersectIndex));
            }
        }
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private float PerpendicularDistance(float3 point1, float3 pointDirection, float3 point2)
        {
            return math.length(point1 - point2) * (1 - math.abs(math.dot(math.normalize(point1 - point2), pointDirection)));
        }
    }
}
