using UnityEngine;

namespace RaftsWar.Boats
{
    public class DamageDealer
    {
        public IDamageable target;
        public float damage;
        public Vector3 pos;
        
        private Team _team;


        public DamageDealer(float damage, Vector3 towerPos, IDamageable target, Team team)
        {
            this.pos = towerPos;
            this.damage = damage;
            this.target = target;
            this._team = team;
        }

        public void DealDamage()
        {
            target?.TakeDamage(new DamageArgs(pos, damage));
        }
        
        public void DealDamage(IDamageable damageable)
        {
            damageable?.TakeDamage(new DamageArgs(pos, damage));
        }

        public bool CompareTeam(ITarget target)
        {
            return target.Team == _team;
        }

    }
}