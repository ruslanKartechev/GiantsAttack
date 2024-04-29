using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class RunawayHuman : AnimatedTarget
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SimpleForwardMover _mover;
        private static readonly int Offset = Animator.StringToHash("Offset");
        private static readonly int Walk = Animator.StringToHash("Walk");

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

        public void PlayScared()
        {
            _animator.Play("Scared");
        }

        public void PlayRun()
        {
            _animator.SetTrigger(Walk);
        }

        public void RandomizeAnimationOffset()
        {
            _animator.SetFloat(Offset, UnityEngine.Random.Range(0f,1f));
        }
    }
}