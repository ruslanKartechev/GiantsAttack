using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class PropHealth : MonoBehaviour, ITarget, IDamageable
    {
        [SerializeField] private float _startHealth;
        [SerializeField] private HealthDisplayBar _displayBar;
        private bool _wasDamaged;
        
        public IDamageable Damageable
        {
            get => this;
            set { }
        }

        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;
        public event Action<IDamageable> OnFirstDamaged;
        
        public bool CanDamage { get; private set; }
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }

        public float HealthPercent
        {
            get => Health / MaxHealth;
            private set{}
        }

        private void Start()
        {
            SetMaxHealth(_startHealth);
            _displayBar.SetHealth(1f);
            HideDisplay();
        }
        
        public void TakeDamage(DamageArgs args)
        {
            if (CanDamage == false)
                return;
            Health -= args.damage;
            if (Health <= 0f)
            {
                CanDamage = false;
                HideDisplay();
                OnDead?.Invoke(this);
                return;
            }
            _displayBar.UpdateHealth(HealthPercent);
            _displayBar.Flick();
            if (!_wasDamaged)
            {
                _wasDamaged = true;
                ShowDisplay();
                OnFirstDamaged?.Invoke(this);
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
        {
            _displayBar.Show();
        }

        public void HideDisplay()
        {
            _displayBar.Hide();
        }
    }
}