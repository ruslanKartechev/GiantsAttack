using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class AnimatedVehicle : AnimatedTarget
    {
        [SerializeField] private ExplosiveVehicle _explodingVehicle;
        [SerializeField] private SimpleForwardMover _mover;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _animSpeed = 1f;
        private static readonly int Speed = Animator.StringToHash("Speed");

        public override Transform Transform => _mover.Movable;

        public override void AnimateMove()
        {
            gameObject.SetActive(true);
            _animator.enabled = true;
            _animator.SetFloat(Speed,_animSpeed);
            _animator.Play("Move");
        }

        public override void StopMovement()
        {
            _animator.enabled = false;
            _mover.Stop();
        }
        
        public override void Move(Action callback = null)
        {
            gameObject.SetActive(true); 
            _mover.Move(callback);
        }

        public override void MoveToPoint(Transform point, float time, Action callback)
        {
            _mover.Move(point, time, callback);
        }

        public override void Explode()
        {
            _explodingVehicle.Explode();
        }

        public override void ExplodeDefaultDirection()
        {
            _explodingVehicle.ExplodeDefaultDirection();
        }

        public override void ExplodeFromCenter(Vector3 center, float force)
        {
            var dir = (Transform.position - center).normalized * force;
            _explodingVehicle.Explode(dir);
        }

        public override void ExplodeInDirection(Vector3 force)
        {
            _explodingVehicle.Explode(force);
        }

        public override void ExplodeWithTorque(Vector3 force, Vector3 torque)
        {
            _explodingVehicle.Explode(force, torque);
        }
    }
}