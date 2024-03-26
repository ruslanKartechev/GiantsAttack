using System;
using System.Collections.Generic;
using GameCore.UI;
using SleepDev.Ragdoll;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterController : MonoBehaviour, IMonster
    {
        [SerializeField] private float _startHealth = 1000;
        //
        [SerializeField] private MonsterMover _mover;
        [SerializeField] private MonsterHealth _health;
        [SerializeField] private Animator _animator;
        [SerializeField] private Ragdoll _ragdoll;
        [SerializeField] private MonsterAnimEventReceiver _eventReceiver;
        [SerializeField] private Transform _grabHand;
        [SerializeField] private BodySectionsManager _sectionsManager;

        public IDamageable Damageable { get; set; }
        
        public IMonsterMover Mover => _mover;
        public IHealth Health => _health;
        public IMonsterAnimEventReceiver AnimEventReceiver => _eventReceiver;
        public event Action<IMonster> OnKilled;
        
        
        
        public void Kill()
        {
            _animator.enabled = false;
            _ragdoll.Activate();
            OnKilled?.Invoke(this);
        }

        public void Init(IBodySectionsUI ui)
        {
            _animator.enabled = true;
            _health.SetMaxHealth(_startHealth);
            _health.ShowDisplay();
            _health.SetDamageable(true);
            ui.Init();
            _sectionsManager.Init(_health, ui);
        }
        

        public void Idle()
        {
            _animator.Play("Idle");
        }

        public void Attack(Transform target)
        {
        }

        public void Roar()
        {
            _animator.Play("Roar");
        }
        

        public void PickAndThrow(IThrowable target, Action onThrowCallback)
        {
            var bahaviour = new PickAndThrowBehaviour(target, this, _animator, _grabHand, onThrowCallback);
        }
        



    }
}