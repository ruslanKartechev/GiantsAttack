using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public struct MoveOnCircleArgs
    {
        public float circleAngleSpeed;
        public float angularSpeed;
        public float maxTiltAngle;
        public float minTiltAngle;
        public float tiltPeriod;
        [Space(10)]
        public float moveToStartPointSpeed;
        public float rotateToStartAngleSpeed;
        public bool moveToStartPoint;
        public bool refreshAngleOnStart;

    }
}