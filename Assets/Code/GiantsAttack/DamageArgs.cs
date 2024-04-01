using UnityEngine;

namespace GiantsAttack
{
    public struct DamageArgs
    {
        public float damage;
        public Vector3 point;
        public Vector3 direction;

        public DamageArgs(float damage, Vector3 point, Vector3 direction)
        {
            this.damage = damage;
            this.point = point;
            this.direction = direction;
        }
    }
}