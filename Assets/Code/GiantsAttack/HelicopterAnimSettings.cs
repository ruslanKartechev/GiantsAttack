using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class HelicopterAnimSettings
    {
        [Header("Vertical")]
        public float time;
        public float maxMagn;
        public AnimationCurve curve;
        [Space(5)]
        [Header("Loitering")]
        public float loiteringMagn;
        public float loiteringMoveSpeed;
        public float loiteringRotationSpeed;
    }
}