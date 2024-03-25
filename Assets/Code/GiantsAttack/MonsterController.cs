using System;
using System.Collections.Generic;
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
        [SerializeField] private List<BodyPartTarget> _bodyParts;

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

        public void Init()
        {
            _animator.enabled = true;
            _health.SetMaxHealth(_startHealth);
            _health.ShowDisplay();
            _health.SetDamageable(true);
            foreach (var target in _bodyParts)
                target.DamageRedirect = _health;
            
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

        public void GrabTarget(IThrowable target, Action callback)
        {
            throw new NotImplementedException();
        }

        public void ThrowAt(IThrowable target, Vector3 targetPoint, Action callback)
        {
            throw new NotImplementedException();
        }

        public void PickAndThrow(IThrowable target, Action onThrowCallback)
        {
            var bahaviour = new PickAndThrowBehaviour(target, this, _animator, _grabHand, onThrowCallback);
        }
        


#if UNITY_EDITOR
        [ContextMenu("E_AddOrGrabAllBodyParts")]
        public void E_AddOrGrabAllBodyParts()
        {
            _bodyParts = new List<BodyPartTarget>(10);
            foreach (var rp in _ragdoll.parts)
            {
                var go = rp.rb.gameObject;
                var part = go.GetComponent<BodyPartTarget>();
                if (part == null)
                    part = go.AddComponent<BodyPartTarget>();
                _bodyParts.Add(part);
                UnityEditor.EditorUtility.SetDirty(go);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}