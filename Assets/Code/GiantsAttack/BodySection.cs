using System.Collections.Generic;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class BodySection
    {
        [SerializeField] private int _sectionID;
        [SerializeField] private float _maxHealth;
        [SerializeField] private List<BodyPartTarget> _targets;
        [SerializeField] private FlickerAnimator _flicker;
        
        private float _health;
        private IHealth _fullBodyHealth;
        private IBodyPartUI _partUI;
        /// <summary>
        /// 0 - green, 1 - yellow, 2 - red
        /// </summary>
        private int _currentHealthLevel = 0;
        
        public int SectionID => _sectionID;

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
            foreach (var tr in _targets)
                tr.CanDamage = canDamage;
        }

        public void Init(IHealth fullBodyHealth)
        {
            _fullBodyHealth = fullBodyHealth;
            SetHealth(_maxHealth);
            foreach (var tr in _targets)
                tr.DamageHandler = DamageCallback;
        }
        
        public void SetUI(IBodySectionsUI ui)
        {
            _partUI = ui.GetBodyPartByID(_sectionID);
        }
        
        
        private void DamageCallback(BodyPartTarget target, DamageArgs args)
        {
            if (Health <= 0)
                return;
            Health -= args.damage;
            var level = 0;
            var percent = _health / _maxHealth;
            if (percent <= .4f)
                level = 2;
            else if (percent <= .8f)
                level = 1;
            
            if (_currentHealthLevel != level)
            {
                _currentHealthLevel = level;
                _partUI.SetDamageLevel(level);
            }
            _fullBodyHealth.TakeDamage(args);
            _partUI.Animate();
            _flicker?.Flick();
            if (Health <= 0)
            {
                _partUI.SetNonDamageable();
                SetDamageable(false);
            }
        }
    }
}