using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class CircularPathBuilder : MonoBehaviour
    {
        public float angleWidth;
        public Transform startPoint;
        [Range(-1,1)] public int angleDir;
        [SerializeField] private CircularPath _path;
        
        public CircularPath Path => _path;
        
        public void Build()
        {
            _path = new CircularPath();
            var root = transform;
            _path.root = root;
            var radVec = (startPoint.position - root.position).XZPlane();
            var startAngle = Vector3.SignedAngle(radVec, root.forward, Vector3.up);
            var endAngle = startAngle;
            if (angleDir < 0)
                endAngle -= angleWidth;
            else
                endAngle += angleWidth;
            _path.mainRadius = radVec.magnitude;
            _path.startAngle = startAngle;
            _path.endAngle = endAngle;
            _path.radVec = root.forward;
        }
        
        
#if UNITY_EDITOR
        public bool doDraw;
        public float radius = .1f;
        public Color color;
        public bool autoBuildOnDraw;

        private void OnDrawGizmos()
        {
            if(autoBuildOnDraw)
                Build();
            if(doDraw)
                Draw();
        }
        

        public void Draw()
        {
            var oldCol = Gizmos.color;
            Gizmos.color = color;
            var oldPos = _path.GetCirclePosAtAngle(_path.startAngle);
            Gizmos.DrawSphere(oldPos, radius);
            var count = 50;
            for (var i = 1f; i <= count; i++)
            {
                var t = i / count;
                var angle = Mathf.Lerp(_path.startAngle, _path.endAngle, t);
                var pos = _path.GetCirclePosAtAngle(angle);
                Gizmos.DrawLine(oldPos, pos);
                oldPos = pos;
            }
            Gizmos.DrawSphere(oldPos, radius);
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