using System;
using UnityEngine;
using SleepDev;

namespace GiantsAttack
{
    public interface IHelicopterMover
    {
        MoverSettings Settings { get; set; }
        void BeginMovingOnCircle(CircularPath path, Transform lookAtTarget, bool loop, Action callback);
        void Evade(EDirection2D direction, Action callback);
        void StopMovement();
        void BeginAnimating();
        void StopAnimating();
        void StopAll();
        public void RotateToLook(Transform lookAt, float time);
        public void StopRotating();

        /// <summary>
        /// Way to animate while standing. Slowly moves locally in XY plane. Replaces Animating routine
        /// </summary>
        /// <param name="lookAt"></param>
        void Loiter(Transform lookAt);
        void StopLoiter();

    }
}