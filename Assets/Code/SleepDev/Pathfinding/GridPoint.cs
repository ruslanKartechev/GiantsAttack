using UnityEngine;

namespace SleepDev
{
    [System.Serializable]
    public struct GridPoint
    {
        public GridPoint(Vector3 worldPos, bool isWalkable)
        {
            WorldPos = worldPos;
            IsWalkable = isWalkable;
        }

        public Vector3 WorldPos;
        public bool IsWalkable;
    }
}