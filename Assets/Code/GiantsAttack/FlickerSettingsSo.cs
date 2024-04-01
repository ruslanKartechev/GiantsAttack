using UnityEngine;

namespace GiantsAttack
{
    [CreateAssetMenu(menuName = "SO/FlickerSettings", fileName = "FlickerSettings", order = 0)]
    public class FlickerSettingsSo : ScriptableObject
    {
        public Color color;
        public float flickTime;
        
    }
}