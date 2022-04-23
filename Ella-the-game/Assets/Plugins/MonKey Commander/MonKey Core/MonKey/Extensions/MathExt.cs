
using System;
using UnityEngine;

namespace MonKey.Extensions
{
    public static class MathExt
    {
        public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
        {
            Vector3 linePointToPoint = point - linePoint;

            float t = Vector3.Dot(linePointToPoint, lineVec);

            return linePoint + lineVec * t;
        }


        public static Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            float distance = SignedDistancePlanePoint(planeNormal, planePoint, point);
            distance *= -1;
            Vector3 translationVector = SetVectorLength(planeNormal, distance);
            return point + translationVector;
        }

        private static Vector3 SetVectorLength(Vector3 vector, float size)
        {
            Vector3 vectorNormalized = Vector3.Normalize(vector);
            return vectorNormalized * size;
        }

        public static float SignedDistancePlanePoint(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
        {
            return Vector3.Dot(planeNormal, (point - planePoint));
        }
    }
}
