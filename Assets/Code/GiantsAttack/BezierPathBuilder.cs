using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class BezierPathBuilder : MonoBehaviour
    {
        [SerializeField] private List<Transform> _points;

        [SerializeField] private List<BezierPathPart> _pathParts;
        
        public BezierPath LastBuilt { get; private set; }
        
        public BezierPath BuildPath()
        {
            var path = new BezierPath(_pathParts);
            LastBuilt = path;
            return path;
        }
        
        public void BuildPathParts()
        {
            if (_points.Count < 3)
                return;
            var ind = 0;
            var loop = true;
            _pathParts = new List<BezierPathPart>(3);
            while (loop)
            {
                var p1 = _points[ind];
                var p2 = _points[ind + 1];
                var p3 = _points[ind + 2];
                _pathParts.Add(new BezierPathPart(p1,p2,p3));
                ind += 2;
                if (_points.Count - ind < 3)
                    loop = false;
            }
         
        }

        #if UNITY_EDITOR
        public bool doDraw;
        public float radius = .1f;
        public Color color;
        public bool autoBuildOnDraw;

        private void OnDrawGizmos()
        {
            if(autoBuildOnDraw)
                BuildPathParts();
            if(doDraw)
                Draw();
        }
        

        public void Draw()
        {
            var oldCol = Gizmos.color;
            Gizmos.color = color;
            foreach (var part in _pathParts)
            {
                var oldPos = part.p1;
                var max = 15f;
                for (var i = 1f; i <= max; i++)
                {
                    var p = Bezier.GetPosition(part.p1, part.p2, part.p3, i / max);
                    Debug.DrawLine(p, oldPos);
                    oldPos = p;
                }
                Gizmos.DrawSphere(part.p1, radius);
                Gizmos.DrawSphere(part.p2, radius);
                Gizmos.DrawSphere(part.p3, radius);
            }
            Gizmos.color = oldCol;
        }

        public void DrawOn()
        {
            doDraw = true;
        }

        public void DrawOff()
        {
            doDraw = false;
        }
        #endif
    }
}