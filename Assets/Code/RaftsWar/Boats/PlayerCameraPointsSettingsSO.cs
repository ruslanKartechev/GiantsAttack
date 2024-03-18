using UnityEngine;

namespace RaftsWar.Boats
{
    [CreateAssetMenu(menuName = "SO/PlayerCameraPointsSettings", fileName = "PlayerCameraPointsSettingsSO", order = 0)]
    public class PlayerCameraPointsSettingsSO : ScriptableObject
    {
        public PlayerCameraPointsSettings settings;
    }
}