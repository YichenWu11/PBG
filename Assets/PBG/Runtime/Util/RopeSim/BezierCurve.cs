using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public static class BezierCurve
    {
        //Update the positions of the rope section
        public static void GetBezierCurve(Vector3 A, Vector3 B, Vector3 C, Vector3 D, List<Vector3> allRopeSections)
        {
            //The resolution of the line
            //Make sure the resolution is adding up to 1, so 0.3 will give a gap at the end, but 0.2 will work
            var resolution = 0.1f;

            //Clear the list
            allRopeSections.Clear();


            float t = 0;

            while (t <= 1f)
            {
                //Find the coordinates between the control points with a Bezier curve
                var newPos = DeCasteljausAlgorithm(A, B, C, D, t);

                allRopeSections.Add(newPos);

                //Which t position are we at?
                t += resolution;
            }

            allRopeSections.Add(D);
        }


        //The De Casteljau's Algorithm
        private static Vector3 DeCasteljausAlgorithm(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t)
        {
            //Linear interpolation = lerp = (1 - t) * A + t * B
            //Could use Vector3.Lerp(A, B, t)

            //To make it faster
            var oneMinusT = 1f - t;

            //Layer 1
            var Q = oneMinusT * A + t * B;
            var R = oneMinusT * B + t * C;
            var S = oneMinusT * C + t * D;

            //Layer 2
            var P = oneMinusT * Q + t * R;
            var T = oneMinusT * R + t * S;

            //Final interpolated position
            var U = oneMinusT * P + t * T;

            return U;
        }
    }
}