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
    public struct CyloopMeshTriangulateSidesJob : IJobParallelFor
    {
        [ReadOnly]
        public int length;
        [WriteOnly]
        public NativeArray<ushort> indexBuffer;
        public void Execute(int i)
        {
            // Writing triangles for the sides
            int indexAdjusted = i * 3;
            indexBuffer[indexAdjusted] = (ushort)i;
            indexBuffer[indexAdjusted + 1] = (ushort)((i + 1) % length);
            indexBuffer[indexAdjusted + 2] = (ushort)((i + 2) % length);
            // Respecting Unity's winding order of making front face triangles clockwise. Not sure if it matters, but ehh might as well
            /*if (i % 2 == 0)
            {
                indexBuffer[indexAdjusted + 1] = (ushort)((i + 1) % points.Length);
                indexBuffer[indexAdjusted + 2] = (ushort)((i + 2) % points.Length);
            }
            else
            {
                indexBuffer[indexAdjusted + 1] = (ushort)((i + 2) % points.Length);
                indexBuffer[indexAdjusted + 2] = (ushort)((i + 1) % points.Length);
            }*/
        }
    }
}
