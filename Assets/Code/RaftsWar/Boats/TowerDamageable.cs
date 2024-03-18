using System;
using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerDamageable : MonoBehaviour, IDamageable
    {
        public event Action<IDamageable> OnDied;
        public event Action OnDamaged;

        [SerializeField] private HealthDisplay _healthDisplay;
        private bool _isDead;
        private bool _shownDisplay;

        public GameObject Go => gameObject;
        public bool CanDamage { get; set; }
        public bool IsDead => _isDead;
        public bool IsAlive => !_isDead;
        public Team Team { get; set; }
        
        protected float MaxHealth { get; set; }
        public float Health { get; set; }
        public float Percent => Health / MaxHealth;


        public void Init(float maxHealth)
        {
            Health = MaxHealth = maxHealth;
        }

        public void SetMaxHealth(float maxHealth)
        {
            var percent = Percent;
            MaxHealth = maxHealth;
            Health = percent * maxHealth;
        }

        public void HideDisplay()
        {
            _healthDisplay.Off();
        }
        
        public void TakeDamage(DamageArgs args)
        {
            if (_isDead)
                return;
            if (_shownDisplay == false)
            {
                _shownDisplay = true;
                _healthDisplay.On();
            }
            // CLog.Log($"[Tower] health left {Health}");
            Health -= args.damage;
            if (Health <= 0)
            {
                Die();
                _healthDisplay.UpdateFill(0f);
                return;
            }
            _healthDisplay.UpdateFill(Percent);
            _healthDisplay.PlayDamaged(args.damage);
            OnDamaged?.Invoke();
        }

        public void SetDisplayPosition(Transform point)
        {
            _healthDisplay.transform.position = point.position;
        }
        
        private void Die()
        {
            _isDead = true;
            _healthDisplay.Off();
            OnDied?.Invoke(this);
        }
    }
}