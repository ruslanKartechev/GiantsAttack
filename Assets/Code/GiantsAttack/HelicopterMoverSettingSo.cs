using UnityEngine;

namespace GiantsAttack
{
    [CreateAssetMenu(menuName = "SO/HelicopterMoverSetting", fileName = "HelicopterMoverSettingSo", order = 0)]
    public class HelicopterMoverSettingSo : ScriptableObject
    {
        public EvasionSettings evasionSettings;
        public MovementSettings movementSettings;
        public HelicopterAnimSettingsSo animSettingsSo;

    }

    [System.Serializable]
    public class MovementSettings
    {
        public float moveToPointSpeed;
        public Vector2 leanAngles;
        public AnimationCurve defaultMoveCurve;
        [Range(0f, 1f)] public float leanRotT = .5f;
    }
    
    [System.Serializable]
    public class EvasionSettings
    {
        public float evadeDistance;
        public Vector2 evadeAngles;
        public float evadeTime;
        [Range(0f,1f)] public float rotToEvadeTimeFraction = .5f;
    }
}