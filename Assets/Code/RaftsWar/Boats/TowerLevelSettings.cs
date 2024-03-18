using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class TowerLevelSettings
    {
        public TowerRaftSettings buildingSettings;
        [Space(10)]
        public float radius;
        [Tooltip("Shooting Animation Speed multiplier")] public float fireRate;
        [Tooltip("Damage from each arrow")] public float unitDamage;
        [Tooltip("Tower health")] public float towerHealth;
    }
}