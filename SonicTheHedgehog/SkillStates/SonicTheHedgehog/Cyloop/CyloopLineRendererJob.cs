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
        //[ReadOnly]
        //public bool log;
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
            var mod = (startIndex - (index - 1)) % points.Length;
            if (mod < 0) mod += points.Length;
            //if (log) Log.Message(index + " -> "+mod);
            return mod;
        }

        /*
         * attempted rewrite to include more line renderer points than actual points. hurt my brain. never figured out how to lerp the velocities of the
         * points to actually smooth the line
        [ReadOnly]
        public NativeArray<CyloopPoint> points;
        [ReadOnly]
        public float3 characterPosition;
        [ReadOnly]
        public float3 characterVelocity;
        [ReadOnly]
        public float lerp;
        [ReadOnly]
        public int startIndex;
        [WriteOnly]
        public NativeArray<Vector3> output;

        public void Execute(int index)
        {
            output[index] = index == 0 ? characterPosition : GetPosition(index);
        }
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private int AdjustIndex(int index)
        {
            return ((int)math.ceil(startIndex - ((index - 1) * (1 / StaticValues.cyloopLinePointsPerPoint)))) % points.Length;
        }
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private float3 GetPosition(int index)
        {
            float offset = (index * (1 / StaticValues.cyloopLinePointsPerPoint)) % 1;
            int indexOffset = offset + lerp >= 1 ? index + 1 : index;
            int posIndex = AdjustIndex(indexOffset);
            if (points[posIndex].index - 1 == startIndex)
            {
                return points[posIndex].position;
            }
            float3 pos1WithMovement = math.lerp(points[posIndex].position, points[posIndex].position + )
            return math.lerp(points[posIndex].position, indexOffset == 1 ? characterPosition : points[AdjustIndex(indexOffset + 1)].position, (offset + lerp) % 1);
        }
         */
    }
}
