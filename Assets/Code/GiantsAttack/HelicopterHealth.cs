using System;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterHealth : MonoBehaviour, IHealth
    {
        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;
        
        
        private float _health;
        private float _maxHealth;
        private bool _canDamage;
        
        public float Health => _health;
        public float MaxHealth => _maxHealth;
        public bool CanDamage => _canDamage;

        public float HealthPercent => Health / MaxHealth;
        
        public void SetMaxHealth(float val)
        {
            _maxHealth = _health = val;
        }

        public void SetDamageable(bool canDamage)
        {
            _canDamage = canDamage;
        }

        public void ShowDisplay()
        { }

        public void HideDisplay()
        { }

        public void TakeDamage(DamageArgs args)
        {
            _health -= args.damage;
            if (_health <= 0)
            {
                OnDead?.Invoke(this);
            }
            else
            {
                OnDamaged?.Invoke(this);
            }
        }
    }
}