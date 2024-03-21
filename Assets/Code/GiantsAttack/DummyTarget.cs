using System;
using UnityEngine;

namespace GiantsAttack
{
    public class DummyTarget : MonoBehaviour, ITarget, IDamageable
    {
        private IDamageable _damageable;
        public IDamageable Damageable
        {
            get => this;
            set => _damageable = value;
        }
        
        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;
        public bool CanDamage => true;
        public void TakeDamage(DamageArgs args)
        {
        }
    }
}