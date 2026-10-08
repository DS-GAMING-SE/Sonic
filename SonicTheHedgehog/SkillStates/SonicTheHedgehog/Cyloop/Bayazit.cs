using Rewired;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Unity.Mathematics;
using static UnityEngine.UIElements.UIR.Allocator2D;

namespace SonicTheHedgehog.SkillStates.Cyloop
{
    /// <summary>
    /// I don't think this works
    /// This class is took from a random Unity forum post which took from the "FarseerUnity" physics engine, which uses Mark Bayazit's decomposition algorithm.
    /// I also have to make it work with self-intersecting polygons, so I'll use another different algorithm to decompose a self-intersecting polygon into several simple polygons,
    /// and then I would decompose each of them into convex polygons.
    /// </summary>

    //From phed rev 36

    /// <summary>
    /// Convex decomposition algorithm created by Mark Bayazit (http://mnbayazit.com/)
    /// For more information about this algorithm, see http://mnbayazit.com/406/bayazit
    /// </summary>
    public static class Bayazit
    {
        private static float3 At(int i, List<float3> vertices)
        {
            int s = vertices.Count;
            Log.Message($"At: {i} {s} -> {(i < 0 ? s - (-i % s) : i % s)}");
            if (vertices.Count == 1) return 0;
            return vertices[i < 0 ? s - (-i % s) : i % s];
            /*int result = i % vertices.Count;
            if (result < 0) result += vertices.Count;
            return result;*/
        }

        private static List<float3> Copy(int i, int j, List<float3> vertices)
        {
            List<float3> p = new List<float3>();
            while (j < i) j += vertices.Count;
            //p.reserve(j - i + 1);
            for (; i <= j; ++i)
            {
                p.Add(At(i, vertices));
            }
            return p;
        }

        /// <summary>
        /// Decompose the polygon into several smaller non-concave polygon.
        /// If the polygon is already convex, it will return the original polygon, unless it is over Settings.MaxPolygonVertices.
        /// Precondition: Counter Clockwise polygon
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static List<List<float3>> ConvexPartition(List<float3> vertices)
        {
            //We force it to CCW as it is a precondition in this algorithm.
            ForceCounterClockWise(vertices);

            List<List<float3>> list = new List<List<float3>>();
            float d, lowerDist, upperDist;
            float3 p;
            float3 lowerInt = new float3();
            float3 upperInt = new float3(); // intersection points
            int lowerIndex = 0, upperIndex = 0;
            List<float3> lowerPoly, upperPoly;

            for (int i = 0; i < vertices.Count; ++i)
            {
                if (Reflex(i, vertices))
                {
                    lowerDist = upperDist = float.MaxValue; // std::numeric_limits<qreal>::max();
                    for (int j = 0; j < vertices.Count; ++j)
                    {
                        // if line intersects with an edge
                        if (Left(At(i - 1, vertices), At(i, vertices), At(j, vertices)) &&
                            RightOn(At(i - 1, vertices), At(i, vertices), At(j - 1, vertices)))
                        {
                            // find the point of intersection
                            p = LineIntersect(At(i - 1, vertices), At(i, vertices), At(j, vertices), At(j - 1, vertices));
                            if (Right(At(i + 1, vertices), At(i, vertices), p))
                            {
                                // make sure it's inside the poly
                                d = SquareDist(At(i, vertices), p);
                                if (d < lowerDist)
                                {
                                    // keep only the closest intersection
                                    lowerDist = d;
                                    lowerInt = p;
                                    lowerIndex = j;
                                }
                            }
                        }

                        if (Left(At(i + 1, vertices), At(i, vertices), At(j + 1, vertices)) &&
                            RightOn(At(i + 1, vertices), At(i, vertices), At(j, vertices)))
                        {
                            p = LineIntersect(At(i + 1, vertices), At(i, vertices), At(j, vertices),
                                                          At(j + 1, vertices));
                            if (Left(At(i - 1, vertices), At(i, vertices), p))
                            {
                                d = SquareDist(At(i, vertices), p);
                                if (d < upperDist)
                                {
                                    upperDist = d;
                                    upperIndex = j;
                                    upperInt = p;
                                }
                            }
                        }
                    }

                    // if there are no vertices to connect to, choose a point in the middle
                    if (lowerIndex == (upperIndex + 1) % vertices.Count)
                    {
                        float3 sp = ((lowerInt + upperInt) / 2);

                        lowerPoly = Copy(i, upperIndex, vertices);
                        lowerPoly.Add(sp);
                        upperPoly = Copy(lowerIndex, i, vertices);
                        upperPoly.Add(sp);
                    }
                    else
                    {
                        double highestScore = 0, bestIndex = lowerIndex;
                        while (upperIndex < lowerIndex) upperIndex += vertices.Count;
                        for (int j = lowerIndex; j <= upperIndex; ++j)
                        {
                            if (CanSee(i, j, vertices))
                            {
                                double score = 1 / (SquareDist(At(i, vertices), At(j, vertices)) + 1);
                                if (Reflex(j, vertices))
                                {
                                    if (RightOn(At(j - 1, vertices), At(j, vertices), At(i, vertices)) &&
                                        LeftOn(At(j + 1, vertices), At(j, vertices), At(i, vertices)))
                                    {
                                        score += 3;
                                    }
                                    else
                                    {
                                        score += 2;
                                    }
                                }
                                else
                                {
                                    score += 1;
                                }
                                if (score > highestScore)
                                {
                                    bestIndex = j;
                                    highestScore = score;
                                }
                            }
                        }
                        lowerPoly = Copy(i, (int)bestIndex, vertices);
                        upperPoly = Copy((int)bestIndex, i, vertices);
                    }
                    list.AddRange(ConvexPartition(lowerPoly));
                    list.AddRange(ConvexPartition(upperPoly));
                    return list;
                }
            }

            // polygon is already convex
            list.Add(vertices);

            //The polygons are not guaranteed to be without collinear points. We remove
            //them to be sure.
            for (int i = 0; i < list.Count; i++)
            {
                //list _= SimplifyTools.CollinearSimplify(list, 0);_}
            }
            //Remove empty vertice collections
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list.Count == 0) list.RemoveAt(i);
            }

            return list;
        }


        private static bool CanSee(int i, int j, List<float3> vertices)
        {
            if (Reflex(i, vertices))
            {
                if (LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices)) &&
                RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices))) return false;
            }
            else
            {
                if (RightOn(At(i, vertices), At(i + 1, vertices), At(j, vertices)) ||
                LeftOn(At(i, vertices), At(i - 1, vertices), At(j, vertices))) return false;

            }

            if (Reflex(j, vertices))
            {

                if (LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices)) &&
               RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices))) return false;
            }

            else
            {

                if (RightOn(At(j, vertices), At(j + 1, vertices), At(i, vertices)) ||
               LeftOn(At(j, vertices), At(j - 1, vertices), At(i, vertices))) return false;
            }

            for (int k = 0; k < vertices.Count; ++k)
            {

                if ((k + 1) % vertices.Count == i || k == i || (k + 1) % vertices.Count == j || k == j)
                {

                    continue; // ignore incident edges
                }

                float3 intersectionPoint;
                if (LineIntersect2(At(i, vertices), At(j, vertices), At(k, vertices), At(k + 1, vertices), out intersectionPoint))
                {

                    return false;
                }

            }

            return true;
        }

        // precondition: ccw
        private static bool Reflex(int i, List<float3> vertices)
        {

            return Right(i, vertices);
        }


        private static bool Right(int i, List<float3> vertices)
        {

            return Right(At(i - 1, vertices), At(i, vertices), At(i + 1, vertices));
        }


        private static bool Left(float3 a, float3 b, float3 c)
        {

            return Area(ref a, ref b, ref c) > 0;
        }


        private static bool LeftOn(float3 a, float3 b, float3 c)
        {

            return Area(ref a, ref b, ref c) >= 0;
        }


        private static bool Right(float3 a, float3 b, float3 c)
        {

            return Area(ref a, ref b, ref c) < 0;
        }


        private static bool RightOn(float3 a, float3 b, float3 c)
        {

            return Area(ref a, ref b, ref c) <= 0;
        }


        private static float SquareDist(float3 a, float3 b)
        {

            float dx = b.x - a.x;
            float dy = b.y - a.y;
            return dx * dx + dy * dy;
        }

        //forces counter clock wise order.
        private static void ForceCounterClockWise(List<float3> vertices)
        {

            if (!IsCounterClockWise(vertices))
            {

                vertices.Reverse();
            }

        }


        private static bool IsCounterClockWise(List<float3> vertices)
        {

            //We just return true for lines
            if (vertices.Count < 3)
                return true;

            return (GetSignedArea(vertices) > 0.0f);
        }


        //gets the signed area.
        private static float GetSignedArea(List<float3> vertices)
        {

            int i;
            float area = 0;

            for (i = 0; i < vertices.Count; i++)
            {

                int j = (i + 1) % vertices.Count;
                area += vertices[i].x * vertices[j].y;
                area -= vertices[i].y * vertices[j].x;
            }
            area /= 2.0f;
            return area;
        }

        //From Mark Bayazit’s convex decomposition algorithm
        private static float3 LineIntersect(float3 p1, float3 p2, float3 q1, float3 q2)
        {

            float3 i = float3.zero;
            float a1 = p2.y - p1.y;
            float b1 = p1.x - p2.x;
            float c1 = a1 * p1.x + b1 * p1.y;
            float a2 = q2.y - q1.y;
            float b2 = q1.x - q2.x;
            float c2 = a2 * q1.x + b2 * q1.y;
            float det = a1 * b2 - a2 * b1;

            if (!FloatEquals(det, 0))
            {

                // lines are not parallel
                i.x = (b2 * c1 - b1 * c2) / det;
                i.y = (a1 * c2 - a2 * c1) / det;
            }
            return i;
        }

        //from Eric Jordan’s convex decomposition library, it checks if the lines a0->a1 and b0->b1 cross.
        //if they do, intersectionPoint will be filled with the point of crossing. Grazing lines should not return true.
        private static bool LineIntersect2(float3 a0, float3 a1, float3 b0, float3 b1, out float3 intersectionPoint)
        {

            intersectionPoint = float3.zero;

            if (float3.Equals(a0, b0) || float3.Equals(a0, b1) || float3.Equals(a1, b0) || float3.Equals(a1, b1))
                return false;

            float x1 = a0.x;
            float y1 = a0.y;
            float x2 = a1.x;
            float y2 = a1.y;
            float x3 = b0.x;
            float y3 = b0.y;
            float x4 = b1.x;
            float y4 = b1.y;

            //AABB early exit
            if (Math.Max(x1, x2) < Math.Min(x3, x4) || Math.Max(x3, x4) < Math.Min(x1, x2))
                return false;

            if (Math.Max(y1, y2) < Math.Min(y3, y4) || Math.Max(y3, y4) < Math.Min(y1, y2))
                return false;

            float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
            float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
            float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
            if (Math.Abs(denom) < Mathf.Epsilon)
            {
                //Lines are too close to parallel to call
                return false;
            }
            ua /= denom;
            ub /= denom;

            if ((0 < ua) && (ua < 1) && (0 < ub) && (ub < 1))
            {

                intersectionPoint.x = (x1 + ua * (x2 - x1));
                intersectionPoint.y = (y1 + ua * (y2 - y1));
                return true;
            }

            return false;
        }


        private static bool FloatEquals(float value1, float value2)
        {

            return Math.Abs(value1 - value2) <= Mathf.Epsilon;
        }


        // Returns a positive number if c is to the left of the line going from a to b. Positive number if point is left,
        //negative if point is right,and 0 if points are collinear.
        private static float Area(float3 a, float3 b, float3 c)
        {

            return Area(ref a, ref b, ref c);
        }


        //returns a positive number if c is to the left of the line going from a to b. Positive number if point is left, negative if point is right, and 0 if points are collinear.
        private static float Area(ref float3 a, ref float3 b, ref float3 c)
        {
            return a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
        }

        //removes all collinear points on the polygon.
        private static List<float3> CollinearSimplify(List<float3> vertices, float collinearityTolerance)
        {

            //We can’t simplify polygons under 3 vertices
            if (vertices.Count < 3)
                return vertices;

            List<float3> simplified = new List<float3>();

            for (int i = 0; i < vertices.Count; i++)
            {

                int prevId = PreviousIndex(vertices, i);
                int nextId = NextIndex(vertices, i);

                float3 prev = vertices[prevId];
                float3 current = vertices[i];
                float3 next = vertices[nextId];

                //If they collinear, continue
                if (Collinear(ref prev, ref current, ref next, collinearityTolerance))
                    continue;

                simplified.Add(current);
            }


            return simplified;
        }


        //gets the previous index.
        private static int PreviousIndex(List<float3> vertices, int index)
        {

            if (index == 0)
            {

                return vertices.Count - 1;
            }

            return index - 1;
        }


        //nexts the index.
        private static int NextIndex(List<float3> vertices, int index)
        {

            if (index == vertices.Count - 1)
            {

                return 0;
            }

            return index + 1;
        }


        private static bool Collinear(ref float3 a, ref float3 b, ref float3 c, float tolerance)
        {

            return FloatInRange(Area(ref a, ref b, ref c), -tolerance, tolerance);
        }


        //checks if a floating point Value is within a specified range of values (inclusive).
        private static bool FloatInRange(float value, float min, float max)
        {

            return (value >= min && value <= max);
        }
    }
}
