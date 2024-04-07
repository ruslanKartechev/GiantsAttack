using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class HelicopterMoveAroundNode
    {
        public float startDelay;
        [Space(10)]
        public bool changeAngle;
        public float angle;
        public float timeToChangeAngle;
        [Space(10)] 
        public bool changeHeight;
        public float height;
        public float timeToChangeHeight;
        [Space(10)] 
        public bool changeRadius;
        public float radius;
        public float timeToChangeRadius;

    }
}