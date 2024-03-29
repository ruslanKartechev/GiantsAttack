using System;
using GameCore.UI;
using SleepDev;
using SleepDev.Ragdoll;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterController : MonoBehaviour, IMonster
    {
        [SerializeField] private float _startHealth = 1000;
        //
        [SerializeField] private Transform _grabHand;
        [SerializeField] private MonsterMover _mover;
        [SerializeField] private MonsterHealth _health;
        [SerializeField] private Animator _animator;
        [SerializeField] private MonsterAnimEventReceiver _eventReceiver;
        [SerializeField] private BodySectionsManager _sectionsManager;
        [SerializeField] private Ragdoll _ragdoll;
        private bool _isDead;
        
        public IDestroyer Destroyer { get; private set; }
        public IDamageable Damageable { get; set; }
        public IMonsterMover Mover => _mover;
        public IHealth Health => _health;
        public IMonsterAnimEventReceiver AnimEventReceiver => _eventReceiver;
        public BodySectionsManager BodySectionsManager => _sectionsManager;
        public Transform Point => transform;

        public event Action<IMonster> OnDefeated;
        
        
        public void Init(IBodySectionsUI ui)
        {
            CLog.Log($"[Monster] Init");
            _animator.enabled = true;
            _health.SetMaxHealth(_startHealth);
            _health.ShowDisplay();
            _health.SetDamageable(true);
            _health.OnDead += OnHealthOut;
            ui.Init();
            _sectionsManager.Init(_health, ui);
            Destroyer = GetComponent<IDestroyer>();
        }

        public void Kill()
        {
            CLog.Log($"{gameObject.name} Kill");
            if (_isDead)
                return;
            _isDead = true;
            Destroyer.DestroyMe();
            _health.HideDisplay();
            _health.SetDamageable(false);
        }

        public void PreKillState()
        {
            CLog.Log($"{gameObject.name} Prekill");
            _animator.Play("Prekill");
            _health.HideDisplay();
            _health.SetDamageable(false);
            OnDefeated?.Invoke(this);

        }
        
        public void Idle()
        {
            _animator.SetTrigger("Idle");
        }


        public void Roar()
        {
            _animator.SetTrigger("Roar");
        }
        
        public void PickAndThrow(IThrowable target, Action onThrowCallback)
        {
            var bahaviour = new PickAndThrowBehaviour(target, this, _animator, _grabHand, onThrowCallback);
        }
        
        private void OnHealthOut(IDamageable obj)
        {
            _health.OnDead -= OnHealthOut;
            PreKillState();
        }

    }
}