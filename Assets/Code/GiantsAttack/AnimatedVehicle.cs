using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class AnimatedVehicle : AnimatedVehicleBase
    {
        [SerializeField] private ExplosiveVehicle _explodingVehicle;
        [SerializeField] private SimpleForwardMover _mover;
        [SerializeField] private Animator _animator;

        public ExplosiveVehicle explodingVehicle
        {
            get => _explodingVehicle;
            set => _explodingVehicle = value;
        }

        public SimpleForwardMover mover
        {
            get => _mover;
            set => _mover = value;
        }
        

        public override void AnimateMove()
        {
            _animator.enabled = true;
            _animator.Play("Move");
        }

        public override void StopMovement()
        {
            _animator.enabled = false;
            _mover.Stop();
        }

        public override Transform Transform => _mover.Movable;
        
        public override void Move(Action callback = null)
        {
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
    }
}