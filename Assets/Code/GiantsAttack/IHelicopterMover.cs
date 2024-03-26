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

        public void MoveTo(Transform point, float time, AnimationCurve curve, Action callback);

        /// <summary>
        /// Way to animate while standing. Slowly moves locally in XY plane. Replaces Animating routine
        /// </summary>
        /// <param name="lookAt">If NULL will be ignored, if not will rotate and maintain lookAt rotation</param>
        void Loiter(Transform lookAt);
        void StopLoiter(bool moveBackToLocal = true);

    }
}