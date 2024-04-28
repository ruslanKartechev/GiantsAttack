using System;
using UnityEngine;

namespace GiantsAttack
{
    public class PunchStaticBehaviour
    {
        private Action _hitCallback;
        private Action _endCallback;
        private IMonsterAnimEventReceiver _eventReceiver;
        private Animator _animator;


        public PunchStaticBehaviour(string key, Action hitCallback, Action endCallback, IMonsterAnimEventReceiver eventReceiver, Animator animator)
        {
            _hitCallback = hitCallback;
            _endCallback = endCallback;
            _eventReceiver = eventReceiver;
            _animator = animator;
            _animator.SetTrigger(key);
            _eventReceiver.EOnPunch += OnPunch;
            _eventReceiver.EOnAnimationOver += OnCompleted;
        }

        private void OnPunch()
        {
            _eventReceiver.EOnPunch -= OnPunch;
            _hitCallback.Invoke();
        }

        private void OnCompleted()
        {
            _eventReceiver.EOnAnimationOver -= OnCompleted;
            _endCallback.Invoke();
        }
    }
}