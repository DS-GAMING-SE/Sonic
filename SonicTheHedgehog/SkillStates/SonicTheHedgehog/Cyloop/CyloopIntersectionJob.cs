using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    public struct CyloopIntersectionJob : IJobParallelFor
    {
        [ReadOnly]
        public float3 characterPosition;
        [ReadOnly]
        public float3 lastPosition;
        public NativeArray<CyloopLine> lines;
        [ReadOnly]
        public float width;
        [ReadOnly]
        public int startIndex;
        [ReadOnly]
        public int ignoreFirstLines;
        [WriteOnly]
        public NativeArray<bool> intersected;

        public void Execute(int index)
        {
            // Don't run if the line is just being spawned or if the line isn't valid
            if (lines[index].HasIntersect() || (startIndex >= index && index >= startIndex - ignoreFirstLines) || !lines[index].IsValid()) return;
            // Get the character's "up" and use that to find the "normal" of the cyloop "plane"
            var normalizedLineDirection = math.normalize(lines[index].lineDirection);
            var up = math.cross(normalizedLineDirection, math.normalize(lastPosition - characterPosition));
            var cyloopLineNormal = math.cross(normalizedLineDirection, up);

            // Getting the position of the character projected on the cyloop plane
            // Will use this position to figure out if it is intersecting with the plane
            var characterOnPlane = ClosestPointOnPlane(lines[index].lineCenterPosition, cyloopLineNormal, characterPosition);// ProjectOnPlane(characterPosition, cyloopLineNormal) + math.dot(lines[index].lineCenterPosition, cyloopLineNormal) * cyloopLineNormal;
            // Check if the character was on one side of the plane last frame and the other side this frame
            // Do this by checking if before and after are opposite signs, aka seeing if multiplying them is negative
            if ((math.dot(cyloopLineNormal, math.normalize(characterPosition - characterOnPlane))) * (math.dot(cyloopLineNormal, math.normalize(lastPosition - characterOnPlane))) < 0)
            {
                var characterPlaneRelativeToLine = characterOnPlane - lines[index].lineCenterPosition;
                // Checking if the character intersecting with the plane is within the width of the plane
                if (math.abs(math.length(characterPlaneRelativeToLine) * math.dot(math.normalize(characterOnPlane), up)) <= width / 2 &&
                    // Checking if the character isn't going beyond the length of the line
                    math.abs(math.length(characterPlaneRelativeToLine) * math.dot(math.normalize(characterOnPlane), normalizedLineDirection)) <= math.length(lines[index].lineDirection) / 2)
                {
                    CyloopLine line = lines[index];
                    line.lineIntersectIndex = startIndex;
                    line.lineIntersectPosition = characterPosition;
                    lines[index] = line;
                    intersected[0] = true;
                    Log.Message($"vert:{math.abs(math.length(characterPlaneRelativeToLine) * math.dot(math.normalize(characterOnPlane), up))}/{width / 2} -- hor:{math.abs(math.length(characterPlaneRelativeToLine) * math.dot(math.normalize(characterOnPlane), normalizedLineDirection))}/{math.length(lines[index].lineDirection) / 2}\n{lines[index].point1.position} {lines[index].point2.position}");
                }
            }
        }
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private float3 ClosestPointOnPlane(float3 planeOffset, float3 planeNormal, float3 point)
        {
            return point + (math.dot(planeOffset - point, planeNormal) * planeNormal);
        }
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        private float3 ProjectOnPlane(float3 vector, float3 planeNormal)
        {
            var dot = math.dot(vector, planeNormal);
            return new float3(vector.x - planeNormal.x * dot,
                vector.y - planeNormal.y * dot,
                vector.z - planeNormal.z * dot);
        }
    }
}
