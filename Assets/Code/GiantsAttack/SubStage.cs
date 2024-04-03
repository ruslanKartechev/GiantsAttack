using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class SubStage
    {
        public ProjectileStageMode mode;
        public GameObject throwTarget;
        public float projectileMoveTime;
        public bool doWalkToTarget;
        public Transform moveToPoint;
        public float enemyMoveTime;
        [Space(4)] 
        public EDirection2D evadeDir;
        public float evadeDistance;
        public float projectileHealth;
        [Space(4)]
        public bool rotateBeforeThrow;
        public float rotateBeforeThrowTime;
    }
}