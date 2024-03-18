using UnityEngine;

namespace RaftsWar.Boats
{
    public struct DamageArgs
    {
        public DamageArgs(Vector3 fromPoint, float damage)
        {
            this.fromPoint = fromPoint;
            this.damage = damage;
        }

        public Vector3 fromPoint;
        public float damage;
        
    }
}