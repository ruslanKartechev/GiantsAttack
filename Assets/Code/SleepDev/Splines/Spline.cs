using System.Collections.Generic;
using UnityEngine;

namespace SleepDev.Splines
{
    [System.Serializable]
    public class Spline
    {
        [SerializeField] private List<SubSpline3> _subSplines = new List<SubSpline3>();

        public List<SubSpline3> SubSplines => _subSplines;

        public static Vector3 GetCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            return (Mathf.Pow((1 - t), 3)) * p0
                   + (3 * Mathf.Pow((1 - t), 2) * t) * p1
                   + (3 * (1 - t) * t * t) * p2
                   + (t * t * t ) * p3;
        }

        public static Vector3 GetCubicBezier(SubSpline3 segment, float t)
        {
            return GetCubicBezier(segment.p0.worldPosition, segment.p1.worldPosition,
                segment.p2.worldPosition, segment.p3.worldPosition, t);
        }
        
    }
}