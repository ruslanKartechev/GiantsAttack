using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class EnemyThrowableFlyingObject : MonoBehaviour, IEnemyThrowWeapon
    {
        [SerializeField] private SimpleThrowable _throwable;
        [SerializeField] private EnemyThrowWeaponHealth _health;
        [SerializeField] private SimpleForwardMover _mover;
        [SerializeField] private Animator _animator;
        private Action _animateCallback;
        
        public GameObject GameObject => gameObject;
        
        public IThrowable Throwable => _throwable;
        
        public ITarget Target => _health;
        
        public IHealth Health => _health;
        
        
        public void MoveTo(Transform point, float time, Action callback)
        {
            _mover.Move(point, time, callback);
        }

        public void AnimateMove(Action onEnd)
        {
            gameObject.SetActive(true);
            _animateCallback = onEnd;
            _animator.enabled = true;
        }

        public void StopAnimate()
        {
            _animator.enabled = false;
        }

        public void EventOnAnimationMoved()
        {
            _animateCallback?.Invoke();
        }
    }
}