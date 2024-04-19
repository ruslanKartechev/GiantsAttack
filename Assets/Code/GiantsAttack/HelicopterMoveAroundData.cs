using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class HelicopterMoveAroundData
    {
        public Transform center;
        public Transform orientation;
        public Transform lookAt;
        
        public float radius;
        public float angle;
        public float height;

        public float radiusCashed;
        public float angleCashed;
        public float heightCashed;
        
        
        public float targetAngle;
        public float targetHeight;
        public float targetRadius;
        
        public float elapsedAngleTime;
        public float elapsedHeightTime;
        public float elapsedRadiusTime;

        public float timeToChangeAngle;
        public float timeToChangeHeight;
        public float timeToChangeRadius;

        public HelicopterMoveAroundData(Transform center, Transform lookAt)
        {
            this.center = center;
            this.lookAt = lookAt;
            this.orientation = center;
        }
        
        public HelicopterMoveAroundData(Transform center, Transform lookAt, Transform orientation)
        {
            this.center = center;
            this.lookAt = lookAt;
            this.orientation = orientation;
        }

        public HelicopterMoveAroundData(float angle, float height, float radius)
        {
            this.angle = angle;
            this.height = height;
            this.radius = radius;
        }

        public void CalculateAngleHeightRadius(Transform me)
        {
            height = (me.position.y - center.position.y);
            var vec = (me.position - center.position).XZPlane();
            angle = Vector3.SignedAngle(orientation.forward,vec, Vector3.up);
            radius = vec.magnitude;
        }

        public void CalculatePositionAndRotation(out Vector3 position, out Quaternion rotation)
        {
            position = center.position + (Quaternion.Euler(0f, angle, 0f) * orientation.forward) * radius;
            position.y += height;
            rotation = Quaternion.LookRotation(lookAt.position - position);
        }
    }
}