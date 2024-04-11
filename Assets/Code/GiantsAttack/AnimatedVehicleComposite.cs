using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class AnimatedVehicleComposite : AnimatedVehicleBase
    {
        [SerializeField] private List<AnimatedVehicleBase> _vehicles;

        public override void AnimateMove()
        {
            foreach (var av in _vehicles)
                av.AnimateMove();
        }

        public override void StopMovement()
        {
            foreach (var av in _vehicles)
                av.StopMovement();
        }

        public override Transform Transform => transform;
        public override void Move(Action callback = null)
        {
            foreach (var av in _vehicles)
                av.Move();
        }

        public override void MoveToPoint(Transform point, float time, Action callback)
        {
            foreach (var av in _vehicles)
                av.MoveToPoint(point, time, callback);
        }

        public override void Explode()
        {
            foreach (var av in _vehicles)
                av.Explode();
        }

        public override void ExplodeDefaultDirection()
        {
            foreach (var av in _vehicles)
                av.ExplodeDefaultDirection();
        }

        public override void ExplodeFromCenter(Vector3 center, float force)
        {
            foreach (var av in _vehicles)
                av.ExplodeFromCenter(center, force);
        }

        public override void ExplodeInDirection(Vector3 force)
        {
            foreach (var av in _vehicles)
                av.ExplodeInDirection(force);
        }
    }
}