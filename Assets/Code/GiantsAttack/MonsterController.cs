using System;
using System.Collections.Generic;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterController : MonoBehaviour, IMonster
    {
        [SerializeField] private Transform _grabHand;
        [SerializeField] private Transform _animRootBone;
        [SerializeField] private Transform _facePoint;
        [SerializeField] private Transform _lookAtPoint;
        [SerializeField] private MonsterMover _mover;
        [SerializeField] private MonsterHealth _health;
        [SerializeField] private Animator _animator;
        [SerializeField] private MonsterAnimEventReceiver _eventReceiver;
        [SerializeField] private BodySectionsManager _sectionsManager;
        [SerializeField] private List<Transform> _damagePoints;
        private IDefeatedBehaviour _defeatedBehaviour;
        private bool _isDead;
        
        public IMonsterDestroyed Destroyer { get; private set; }
        public IDamageable Damageable { get; set; }
        public IMonsterMover Mover => _mover;
        public IHealth Health => _health;
        public IMonsterAnimEventReceiver AnimEventReceiver => _eventReceiver;
        public BodySectionsManager BodySectionsManager => _sectionsManager;
        public Transform Point => transform;
        public Transform CameraFacePoint => _facePoint;
        public Transform LookAtPoint => _lookAtPoint;
        public List<Transform> DamagePoints => _damagePoints;

        public event Action<IMonster> OnDefeated;
        
        
        public void Init(IBodySectionsUI ui, float maxHealth)
        {
            CLog.Log($"[Monster] Init");
            _animator.enabled = true;
            _health.SetMaxHealth(maxHealth);
            _health.ShowDisplay();
            _health.SetDamageable(true);
            _health.OnDead += OnHealthOut;
            ui.Init();
            _sectionsManager.Init(_health, ui);
            Destroyer = GetComponent<IMonsterDestroyed>();
            _defeatedBehaviour = GetComponent<IDefeatedBehaviour>();
        }

        public void SetMoveAnimationSpeed(float speed)
        {
            _mover.MoveAnimationSpeed = speed;
        }

        public void PunchStatic(string key, Action onHit, Action onCompleted)
        {
            var behaviour = new PunchStaticBehaviour(key, onHit, onCompleted, _eventReceiver, _animator);
        }

        public void Punch(string key, Action punchStartedCallback, Action onPunch, Action onAnimationEnd)
        {
            var s = new PunchBehaviour(this, _animator, key, 
                punchStartedCallback, onPunch, onAnimationEnd);
        }

        public void Kill(bool chopped = false)
        {
            CLog.Log($"{gameObject.name} Kill");
            if (_isDead)
                return;
            _isDead = true;
            _health.SetDamageable(false);
            _health.HideDisplay();
            if(chopped)
                Destroyer.DestroyMeChopped();
            else
                Destroyer.DestroyMe();
        }

        public void PreKillState()
        {
            CLog.Log($"{gameObject.name} Prekill");
            _mover.StopMovement();
            _health.HideDisplay();
            _health.SetDamageable(false);
            _defeatedBehaviour.Play(() => {});
        }

        public void Animate(string key, bool trigger)
        {
            if(trigger)
                _animator.SetTrigger(key);
            else
                _animator.Play(key);
        }

        public void Idle()
        {
            CLog.LogRed("[Enemy] Idle");
            _animator.SetTrigger("Idle");
        }


        public void Roar()
        {
            CLog.LogRed("[Enemy] Roar");
            _animator.SetTrigger("Roar");
        }

        public void Jump(bool transition)
        {
            CLog.LogRed("[Enemy] Jump");
            if(transition)
                _animator.SetTrigger("Jump");
            else
                _animator.Play("Jump");
        }

        public void KickUp()
        {
            CLog.LogRed("[Enemy] KickUp");
            _animator.Play("KickUp");
        }
        
        public void PickAndThrow(IThrowable target,Action onPickCallback, Action onThrowCallback, bool pickFromTop)
        {
            var bahaviour = new PickAndThrowBehaviour(target, this, _animator, _grabHand, 
                onPickCallback, onThrowCallback, pickFromTop);
        }

        public void AlignPositionToAnimRootBone(bool playIdle)
        {
            var pos = _animRootBone.position;
            pos.y = transform.position.y;
            var parent = _animRootBone.parent;
            _animRootBone.parent = null;
            transform.position = pos;
            _animRootBone.parent = parent;
            if(playIdle)
                _animator.Play("Idle");
            Debug.DrawLine(pos, pos + Vector3.up * 100, Color.red, 10f);
        }
        
        private void OnHealthOut(IDamageable obj)
        {
            _health.OnDead -= OnHealthOut;
            OnDefeated?.Invoke(this);
        }
    }
}