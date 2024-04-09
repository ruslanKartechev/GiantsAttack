using System;
using UnityEngine;

namespace GiantsAttack
{
    public abstract class AnimatedVehicleBase : MonoBehaviour
    {
        public abstract void AnimateMove();
        public abstract void StopMovement();
        public abstract Transform Transform { get; }

        /// <summary>
        /// Moves to a default preset point
        /// </summary>
        public abstract void Move(Action callback = null);

        public abstract void MoveToPoint(Transform point, float time, Action callback);
        public abstract void Explode();
        public abstract void ExplodeDefaultDirection();
        public abstract void ExplodeFromCenter(Vector3 center, float force);
        public abstract void ExplodeInDirection(Vector3 force);
    }
}