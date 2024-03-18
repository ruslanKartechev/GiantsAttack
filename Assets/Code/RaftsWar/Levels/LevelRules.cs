using UnityEngine;

namespace RaftsWar.Levels
{
    [CreateAssetMenu(menuName = "SO/LevelRules", fileName = "LevelRules", order = 0)]
    public class LevelRules : ScriptableObject
    {
        public static float PlayerCameraSetTime = 1f;
        public static int PlayerBoatMaxConnectionsCount =8;

    }
}