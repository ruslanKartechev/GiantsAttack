using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class PlayerCameraPointsSettings
    {
        public List<float> distances;
        public float angle;

        public List<Vector3> ConvertToPoints()
        {
            var points = new List<Vector3>(distances.Count);
            var dir = Quaternion.Euler(angle, 0f, 0) * Vector3.up;
            foreach (var d in distances)
            {
                points.Add(dir * d);
            }
            return points;
        }
    }
}