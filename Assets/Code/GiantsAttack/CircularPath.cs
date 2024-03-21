using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class CircularPath
    {
        public Transform root;
        public Vector3 radVec;
        public float mainRadius;
        public float startAngle;
        public float endAngle;
        
        public Vector3 GetCirclePosAtAngle(float angle)
        {
            return root.position + Quaternion.Euler(0, angle, 0)
                * (root.forward * mainRadius);
        }
        
    }
}