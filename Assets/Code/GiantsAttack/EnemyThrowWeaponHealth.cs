using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class EnemyThrowWeaponHealth : MonoBehaviour, ITarget, IHealth
    {
        public IDamageable Damageable
        {
            get => this;
            set { }
        }

        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;
        
        public bool CanDamage { get; private set; }
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public float HealthPercent { get; private set; }
        
        public void TakeDamage(DamageArgs args)
        {
            if (CanDamage == false)
                return;
            Health -= args.damage;
            if (Health <= 0f)
            {
                CanDamage = false;
                OnDead?.Invoke(this);
            }
        }

        public void SetMaxHealth(float val)
        {
            MaxHealth = Health = val;
        }

        public void SetDamageable(bool canDamage)
        {
            CanDamage = canDamage;
        }

        public void ShowDisplay()
        { }

        public void HideDisplay()
        { }
    }
}