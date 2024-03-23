using System;
using UnityEngine;
using SleepDev;

namespace GiantsAttack
{
    public class MonsterHealth : MonoBehaviour, IHealth
    {
        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;

        [SerializeField] private HealthDisplayBar _healthBar;
        private float _health;
        private float _maxHealth;
        private bool _canDamage;
        
        public bool CanDamage => _canDamage;
        public float Health => _health;
        public float MaxHealth => _maxHealth;
        public float HealthPercent => _health / _maxHealth;
        
        public void TakeDamage(DamageArgs args)
        {
            if (_canDamage == false)
                return;
            _health -= args.damage;
            _healthBar.UpdateHealth(HealthPercent);
            if (_health <= 0)
            {
                _health = 0f;
                OnDead?.Invoke(this);
                return;
            }
            _healthBar.UpdateHealth(HealthPercent);
            OnDamaged?.Invoke(this);
        }

        public void SetMaxHealth(float val)
        {
            _health = _maxHealth = val;
            _healthBar.SetHealth(HealthPercent);
        }

        public void SetDamageable(bool canDamage)
        {
            _canDamage = canDamage;
        }

        public void ShowDisplay()
        {
            _healthBar.Show();
            _healthBar.SetHealth(HealthPercent);
        }

        public void HideDisplay()
        {
            _healthBar.Hide();
        }
    }
}