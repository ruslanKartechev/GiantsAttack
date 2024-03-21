using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class BezierPath
    {
        private List<BezierPathPart> _parts;
        private float _t;
        private float _totalLength;

        public float TotalLength => _totalLength;
        public float T => _t;
        
        public BezierPath(List<BezierPathPart> parts)
        {
            _parts = parts;
            foreach (var p in _parts)
            {
                _totalLength += p.length;
            }
        }

        public Vector3 GetPositionAt(float t)
        {
            return Vector3.zero;
        }
        
    }
}