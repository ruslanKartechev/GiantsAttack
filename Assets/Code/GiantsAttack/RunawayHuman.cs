using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class RunawayHuman : AnimatedTarget
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SimpleForwardMover _mover;
        
        public override void AnimateMove()
        {
            gameObject.SetActive(true);
            _animator.enabled = true;
            _mover.Move(Hide);
        }

        public override void StopMovement()
        {
        }

        public override Transform Transform => transform;
        
        public override void Move(Action callback = null)
        {
            gameObject.SetActive(true);
            _animator.enabled = true;
            _mover.Move(callback);
        }

        public override void MoveToPoint(Transform point, float time, Action callback)
        {
            gameObject.SetActive(true);
            _animator.enabled = true;
            _mover.Move(point, time, callback);
        }

        public override void Explode()
        {
            Hide();
        }

        public override void ExplodeDefaultDirection()
        {
        }

        public override void ExplodeFromCenter(Vector3 center, float force)
        {
        }

        public override void ExplodeInDirection(Vector3 force)
        {
        }

        public override void ExplodeWithTorque(Vector3 force, Vector3 torque)
        {
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}