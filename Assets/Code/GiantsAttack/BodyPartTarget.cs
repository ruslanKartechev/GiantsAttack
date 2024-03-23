using System;
using UnityEngine;

namespace GiantsAttack
{
    public class BodyPartTarget : MonoBehaviour, ITarget, IDamageable
    {
        /// <summary>
        /// Used to redirect damage to the Host Entity.
        /// </summary>
        public IDamageable DamageRedirect { get; set; }
        
        /// <summary>
        /// Returns itself
        /// </summary>
        public IDamageable Damageable
        {
            get => this; set{} }

        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;
        
        public bool CanDamage => DamageRedirect.CanDamage;
        
        public void TakeDamage(DamageArgs args)
        {
            DamageRedirect.TakeDamage(args);
        }
    }
}