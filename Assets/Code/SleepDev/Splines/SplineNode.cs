using UnityEngine;

namespace SleepDev.Splines
{
    [System.Serializable]
    public struct SplineNode
    {
        public SplineNode(Vector3 worldPosition, Quaternion worldRotation)
        {
            this.worldPosition = worldPosition;
            this.worldRotation = worldRotation;
        }

        public Vector3 worldPosition;
        public Quaternion worldRotation;
    }
}