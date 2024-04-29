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
        [SerializeField] private Transform _facePoint;
        [SerializeField] private Transform _lookAtPoint;
        [SerializeField] private Transform _killPoint;
        [SerializeField] private BodyArmorManager _armorManager;
        [SerializeField] private SkinnedMeshRenderer _mainRenderer;
        [Space(10)]
        [SerializeField] private MonsterAnimEventReceiver _eventReceiver;
        [SerializeField] private BodySectionsManager _sectionsManager;
        [SerializeField] private MonsterHealth _health;
        [SerializeField] private MonsterMover _mover;
        [SerializeField] private Animator _animator;
        [SerializeField] private ChoppedMeshSpawner _choppedMeshSpawner;
        [SerializeField] private List<Transform> _damagePoints;
        private IDefeatedBehaviour _defeatedBehaviour;
        private bool _isDead;
        
        public IMonsterDestroyed Destroyer { get; private set; }
        public IDamageable Damageable { get; set; }
        public IMonsterMover Mover => _mover;
        public IHealth Health => _health;
        public IMonsterAnimEventReceiver AnimEventReceiver => _eventReceiver;
        public BodySectionsManager BodySectionsManager => _sectionsManager;
        public Transform Point => _animator.transform;
        public Transform CameraFacePoint => _facePoint;
        public Transform LookAtPoint => _lookAtPoint;
        public Transform KillPoint => _killPoint;
        public List<Transform> DamagePoints => _damagePoints;

        public event Action<IMonster> OnDefeated;
        
        
        public void Init(IBodySectionsUI ui, float maxHealth, EnemyView view)
        {
            CLog.Log($"[Monster] Init");
            view.SetView(_mainRenderer);
            _choppedMeshSpawner.View = view;
            
            _animator.enabled = true;
            _health.SetMaxHealth(maxHealth);
            _health.ShowDisplay();
            _health.SetDamageable(true);
            _health.OnDead += OnHealthOut;
            ui.Init();
            _sectionsManager.Init(_health, ui);
            Destroyer = GetComponent<IMonsterDestroyed>();
            _defeatedBehaviour = GetComponent<IDefeatedBehaviour>();
            _armorManager.SpawnArmor();
        }

        public void Scale(float scale)
        {
            _animator.transform.localScale *= scale;
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
            _animator.SetTrigger("Idle");
        }


        public void Roar()
        {
            _animator.SetTrigger("Roar");
        }

        public void Jump(bool transition)
        {
            if(transition)
                _animator.SetTrigger("Jump");
            else
                _animator.Play("Jump");
        }

        public void KickUp()
        {
            _animator.Play("KipUp");
        }
        
        public void PickAndThrow(IThrowable target,Action onPickCallback, Action onThrowCallback, bool pickFromTop)
        {
            var bahaviour = new PickAndThrowBehaviour(target, this, _animator, _grabHand, 
                onPickCallback, onThrowCallback, pickFromTop);
        }

        public void SetArmorData(List<ArmorDataSo> armors)
        {
            _armorManager.ArmorData = armors;
        }

        public void SpecialAttack(string id, Action attackStartCallback, Action attackCallback, Action endCallback)
        {
            if (!gameObject.TryGetComponent<IMonsterSpecialAttack>(out var specialAttack))
            {
                Debug.LogError($"{gameObject.name} IMonsterSpecialAttack not found");
                return;
            }
            specialAttack.Attack(id, attackStartCallback, attackCallback, endCallback);
        }

        private void OnHealthOut(IDamageable obj)
        {
            _health.OnDead -= OnHealthOut;
            OnDefeated?.Invoke(this);
        }
    }

    public interface IMonsterSpecialAttack
    {
        void Attack(string id, Action punchStartedCallback, Action attackCallback, Action endCallback);
    }
}