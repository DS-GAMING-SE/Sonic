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
    public struct CyloopLineRendererJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<CyloopPoint> points;
        [ReadOnly]
        public float3 characterPosition;
        [ReadOnly]
        public float lerp;
        [ReadOnly]
        public int startIndex;
        [WriteOnly]
        public NativeArray<Vector3> output;

        public void Execute(int index)
        {
            if (index == 0)
            {
                output[index] = characterPosition;
            }
            else if (index == 1)
            {
                output[index] = math.lerp(points[AdjustIndex(index)].position, characterPosition, lerp);
            }
            else
            {
                output[index] = math.lerp(points[AdjustIndex(index)].position, points[AdjustIndex(index + 1)].position, lerp);
            }
        }
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private int AdjustIndex(int index)
        {
            return (startIndex - (index - 1)) % points.Length;
        }
    }
}
