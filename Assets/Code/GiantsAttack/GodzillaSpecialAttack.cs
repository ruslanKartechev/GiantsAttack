using System;
using UnityEngine;

namespace GiantsAttack
{
    public class GodzillaSpecialAttack : MonoBehaviour, IMonsterSpecialAttack
    {
        [SerializeField] private ParticleSystem _fireParticles;
        [SerializeField] private Animator _animator;
        [SerializeField] private MonsterAnimEventReceiver _eventReceiver;
        private Action _punchStartedCallback;
        private Action _attackCallback;
        private Action _endCallback;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_animator == null)
                _animator = GetComponentInChildren<Animator>();
            if (_eventReceiver == null)
                _eventReceiver = GetComponentInChildren<MonsterAnimEventReceiver>();
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        public void Attack(string id, Action punchStartedCallback, Action attackCallback, Action endCallback)
        {
            _punchStartedCallback = punchStartedCallback;
            _attackCallback = attackCallback;
            _endCallback = endCallback;
            _eventReceiver.OnPunchBegan -= StartAttack;
            _eventReceiver.OnPunch -= MakeAttack;
            _eventReceiver.OnAnimationOver -= EndAttack;
            _eventReceiver.OnPunchBegan += StartAttack;
            _eventReceiver.OnPunch += MakeAttack;
            _eventReceiver.OnAnimationOver += EndAttack;
            _animator.SetTrigger(id);
        }

        private void EndAttack()
        {
            _fireParticles.Stop();
            _endCallback.Invoke();
        }

        private void MakeAttack()
        {
            _attackCallback.Invoke();
        }

        private void StartAttack()
        {
            _punchStartedCallback.Invoke();
            _fireParticles.gameObject.SetActive(true);
            _fireParticles.Play();
        }
    }
}