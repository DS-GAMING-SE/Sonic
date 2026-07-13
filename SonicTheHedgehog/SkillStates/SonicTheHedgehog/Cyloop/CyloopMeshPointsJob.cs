using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public struct CyloopMeshPointsJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<CyloopPoint> points;
        [ReadOnly]
        public float width;
        [WriteOnly]
        public NativeArray<float3> output;
        public void Execute(int i)
        {
            // do something with figuring out up vector?
            if (i % 2 == 0)
            {
                output[i] = points[i / 2].position - new float3(0f, width / 2, 0f);
            }
            else
            {
                output[i] = points[i / 2].position + new float3(0f, width / 2, 0f);
            }
        }
    }
}
