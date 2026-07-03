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
        public NativeArray<int> newIntersect;

        public void Execute(int index)
        {
            // Don't run if the line has already been intersected or if the line is just being spawned or if the line isn't valid
            // Should no repeat intersect be a rule?
            if (lines[index].HasIntersect() || (startIndex >= index && index >= startIndex - ignoreFirstLines) || !lines[index].IsValid()) return;

            // The cyloop trail is just a bunch of points connected together to make a line. It's just a line with no width, so intersecting it..
            // ..would be basically impossible. The goal here is to use vector math to turn the cyloop line into a plane that is..
            // ..always facing towards the player, see if the player's position last frame and this frame go across the plane, then see..
            // ..if the player's position doesn't go beyond the bounds of the plane.

            // Get the character's "up" and use that to find the "normal" of the cyloop plane
            var normalizedLineDirection = math.normalize(lines[index].lineDirection);
            var up = math.normalize(math.cross(normalizedLineDirection, math.normalize(lastPosition - characterPosition)));
            var cyloopLineNormal = math.normalize(math.cross(normalizedLineDirection, up));

            // Getting the position of the character projected on the cyloop plane
            // Will use this position to figure out if it is intersecting with the plane
            var characterOnPlane = ClosestPointOnPlane(lines[index].lineCenterPosition, cyloopLineNormal, characterPosition);// ProjectOnPlane(characterPosition, cyloopLineNormal) + math.dot(lines[index].lineCenterPosition, cyloopLineNormal) * cyloopLineNormal;
            // First, check if the character was or is now directly on the plane
            // If not, then check if the character was on one side one frame, and the other side the other frame, aka before and after are opposite signs, aka multiplying is negative
            if (math.length(characterOnPlane - characterPosition) <= 0.005f || math.length(characterOnPlane - lastPosition) <= 0.005f ||
                (math.dot(cyloopLineNormal, math.normalize(characterPosition - characterOnPlane))) * (math.dot(cyloopLineNormal, math.normalize(lastPosition - characterOnPlane))) < 0)
            {
                var characterPlaneRelativeToLine = characterOnPlane - lines[index].lineCenterPosition;
                // Checking if the character intersecting with the plane is within the width of the plane
                if (math.abs(math.length(characterPlaneRelativeToLine) * math.dot(math.normalize(characterPlaneRelativeToLine), up)) <= width / 2 &&
                    // Checking if the character isn't going beyond the length of the line
                    math.abs(math.length(characterPlaneRelativeToLine) * math.dot(math.normalize(characterPlaneRelativeToLine), normalizedLineDirection)) <= math.length(lines[index].lineDirection) / 2)
                {
                    // Intercept success
                    CyloopLine line = lines[index];
                    line.lineIntersectIndex = startIndex;
                    line.lineIntersectPosition = characterPosition;
                    lines[index] = line;
                    newIntersect[0] = index;
                    Log.Message($"---------------intersect----------------\nnormal:{cyloopLineNormal} up:{up} direction:{normalizedLineDirection} \nvert:{math.abs(math.length(characterPlaneRelativeToLine) * math.dot(math.normalize(characterOnPlane), up))}/{width / 2} -- hor:{math.abs(math.length(characterPlaneRelativeToLine) * math.dot(math.normalize(characterOnPlane), normalizedLineDirection))}/{math.length(lines[index].lineDirection) / 2}\nline points:{lines[index].point1.position} {lines[index].point2.position} index:{index}\ncharacter pos:{characterPosition} plane:{characterOnPlane}");
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
