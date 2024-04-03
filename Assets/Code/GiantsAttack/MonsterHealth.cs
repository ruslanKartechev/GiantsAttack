using System;
using System.Collections.Generic;
using UnityEngine;
using SleepDev;

namespace GiantsAttack
{
    public class MonsterHealth : MonoBehaviour, IHealth
    {
        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;

        [SerializeField] private HealthDisplayBar _healthBar;
        [SerializeField] private List<ParticleSystem> _bloodParticles;
        private int _bloodPartsInd;
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
            if (_health <= 0)
            {
                _health = 0f;
                _canDamage = false;
                OnDead?.Invoke(this);
                return;
            }
            _healthBar.Flick();
            _healthBar.UpdateHealth(HealthPercent);
            //
            var bp = _bloodParticles[_bloodPartsInd];
            bp.transform.position = args.point;
            bp.transform.rotation = Quaternion.LookRotation(-args.direction);
            bp.gameObject.SetActive(true);
            bp.Play();
            _bloodPartsInd++;
            if (_bloodPartsInd >= _bloodParticles.Count)
                _bloodPartsInd = 0;
            //
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