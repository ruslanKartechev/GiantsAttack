using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class AimerSettings
    {
        [Header("UI")]
        public float sensitivityUI;
        public float aimCenterSpeed;
        public float aimUIMaxDiv; 
        public float aimRotSpeed;
        [Header("Body Rotation")]
        public float sensitivity;
        public float rotCenterSpeed;
        public float noiseMagnitude;

    }
}