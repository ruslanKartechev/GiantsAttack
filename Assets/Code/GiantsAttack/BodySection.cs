using System;
using System.Collections.Generic;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class BodySection : IDamageable
    {
        [SerializeField] private int _sectionID;
        [SerializeField] private float _maxHealth;
        [SerializeField] private List<BodyPartTarget> _targets;
        
        private float _health;
        private IHealth _fullBodyHealth;
        private IBodyPartUI _partUI;
        /// <summary>
        /// 0 - green, 1 - yellow, 2 - red
        /// </summary>
        private int _currentHealthLevel = 0;
        private float _damageMult = 1f;

        public int SectionID => _sectionID;
        public float DamageMult => _damageMult;
        public bool IsHead => _sectionID == 0;

        public List<BodyPartTarget> targets => _targets;

        public float Health
        {
            get => _health;
            set => _health = value;
        }

        public float MaxHealth
        {
            get => _maxHealth;
            set => _maxHealth = value;
        }

        public void SetHealth(float health)
        {
            Health = MaxHealth = health;
        }

        public void SetDamageable(bool canDamage)
        {
            CanDamage = canDamage;
        }

        public void Init(IHealth fullBodyHealth)
        {
            _fullBodyHealth = fullBodyHealth;
            SetHealth(_maxHealth);
            foreach (var partTarget in _targets)
                partTarget.Damageable = this;
        }
        
        public void SetUI(IBodySectionsUI ui)
        {
            _partUI = ui.GetBodyPartByID(_sectionID);
            _partUI.SetDamageLevel(_currentHealthLevel);
        }
        
        public event Action<IDamageable> OnDead;
        public event Action<IDamageable> OnDamaged;

        public bool CanDamage { get; private set; } = true;
        
        public void TakeDamage(DamageArgs args)
        {
            args.damage *= _damageMult;
            Health -= args.damage;
            _partUI.Animate();
            if (_health > 0)
            {
                var level = 0;
                var percent = _health / _maxHealth;
                if (percent <= .4f)
                {
                    level = 2;
                    _damageMult = 1.5f;
                }
                else if (percent <= .8f)
                {
                    level = 1;
                    _damageMult = 1.25f;
                }
                if (_currentHealthLevel != level)
                {
                    _currentHealthLevel = level;
                    _partUI.SetDamageLevel(level);
                }
            }
            _fullBodyHealth.TakeDamage(args);
        }
    }
}