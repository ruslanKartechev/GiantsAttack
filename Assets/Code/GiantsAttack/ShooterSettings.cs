using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public struct ShooterSettings
    {
        public float fireDelay;
        public float speed;
        public UnityEngine.Vector2 damage;
        [Header("Crits")]
        public float critChance;
        public float critDamage;
    }
}