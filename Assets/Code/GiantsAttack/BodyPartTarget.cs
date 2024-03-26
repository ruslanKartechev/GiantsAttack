using System;
using UnityEngine;

namespace GiantsAttack
{
    public class BodyPartTarget : MonoBehaviour, ITarget, IDamageable
    {
        public Action<BodyPartTarget, DamageArgs> DamageHandler { get; set; }
        /// <summary>
        /// Returns itself
        /// </summary>
        public IDamageable Damageable
        {
            get => this; set {}
        }

        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;
        
        public bool CanDamage { get; set; } = true;
        
        public void TakeDamage(DamageArgs args)
        {
            DamageHandler.Invoke(this, args);
        }
    }
}