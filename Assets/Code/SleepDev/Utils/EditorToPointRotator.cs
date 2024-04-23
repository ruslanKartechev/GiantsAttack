using UnityEngine;

namespace SleepDev.Utils
{
    public class EditorToPointRotator : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool doWork;
        public Transform toPoint;
        public bool addOffset;
        public float offset;
        
        public void SetRotation()
        {
            if (!doWork || toPoint == null)
                return;
            var p1 = transform.position;
            var p2 = toPoint.position;
            if (addOffset)
                p2.y += offset;
            transform.rotation = Quaternion.LookRotation(p2 - p1);
            UnityEditor.EditorUtility.SetDirty(transform);
        }
#endif
    }
}