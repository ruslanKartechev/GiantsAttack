using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class DamagePointsProvider : IDamagePointsProvider
    {
        [SerializeField] private List<Transform> _points = new List<Transform>(20);

        public void ClearPoint()
        {
            _points.Clear();
        }

        public void AddPoint(Transform point)
        {
            _points.Add(point);
        }

        public void AddPoints(IList<Transform> points)
        {
            _points.AddRange(points);
        }

        public Transform GetClosestTarget(Vector3 sourcePoint)
        {
            return _points.Random();
        }

        public Transform GetRandomTarget()
        {
            return _points.Random();
        }
    }
}