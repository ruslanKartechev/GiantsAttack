using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class BuildingPushable : AnimatedTarget
    {
        [SerializeField] private ExplosiveVehicle _explodingVehicle;

        public override Transform Transform => transform;

        public override void AnimateMove()
        { }
        
        public override void StopMovement()
        { }


        public override void Move(Action callback = null)
        { }

        public override void MoveToPoint(Transform point, float time, Action callback)
        { }
        
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