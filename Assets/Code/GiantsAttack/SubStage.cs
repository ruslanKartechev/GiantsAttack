using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class SubStage
    {
        public Mode mode;
        public GameObject throwTarget;
        public float projectileMoveTime;
        public bool doWalkToTarget;
        public Transform moveToPoint;
        public float enemyMoveTime;
        public float evadeDistance;
        public float projectileHealth;
        [Space(4)]
        public bool rotateBeforeThrow;
        public float rotateBeforeThrowTime;
    }
}