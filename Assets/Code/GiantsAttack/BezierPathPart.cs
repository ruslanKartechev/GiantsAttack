using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class BezierPathPart
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public float length;
        
        public BezierPathPart(){}

        public BezierPathPart(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            CalculateLength();
        }

        public BezierPathPart(Transform p1, Transform p2, Transform p3)
        {
            this.p1 = p1.position;
            this.p2 = p2.position;
            this.p3 = p3.position;
            CalculateLength();
        }

        public void CalculateLength()
        {
            length = SleepDev.Bezier.GetLength(p1, p2, p3, 50);

        }
    }
}