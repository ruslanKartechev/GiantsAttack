using System;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatHealth : MonoBehaviour, IDamageable
    {
        public event Action OnDamaged;
        public event Action<IDamageable> OnDied;

        [SerializeField] private HealthDisplay _healthDisplay;
        private DamageArgs _lastArgs;
        private bool _isDead;
        
        public DamageArgs LastDamagedArgs => _lastArgs;
        public float MaxHealth { get; set; }
        public float Health { get; set; }
        public float Percent => Health / MaxHealth;
        public float Percent100 => Health / MaxHealth * 100f;
        
        public bool IsDead => _isDead;
        public bool IsAlive => !_isDead;

        public GameObject Go => gameObject;
        public bool CanDamage { get; set; }

        public void DisplayOn()
        {
            _healthDisplay.On();
        }
        
        public void DisplayOff()
        {
            _healthDisplay.Off();
        }

        public void Init(float maxHealth)
        {
            MaxHealth = maxHealth;
            Health = maxHealth;
            _healthDisplay.SetFill(Percent);
        }
        
        public void TakeDamage(DamageArgs args)
        {
            if (_isDead)
                return;
            _lastArgs = args;
            Health -= args.damage;
            OnDamaged?.Invoke();
            if (Health <= 0)
            {
                Die();
                return;
            }
            _healthDisplay.UpdateFill(Percent);
            _healthDisplay.PlayDamaged(args.damage);
        }

        [ContextMenu("Health_Die")]
        public void Die()
        {
            CLog.Log($"[BoatHealth] Dead");
            _isDead = true;
            _healthDisplay.Off();
            OnDied?.Invoke(this);
        }
    }
}