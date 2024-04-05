using System;
using UnityEngine;
using SleepDev;

namespace GiantsAttack
{
    public interface IHelicopterMover
    {
        void BeginMovingOnCircle(CircularPath path, Transform lookAt, MoveOnCircleArgs args);
        
        void Evade(EDirection2D direction, Action callback, float evadeDistance);
        void StopMovement();
        void BeginAnimatingVertically();
        void StopAnimating();
        void StopAll();
        public void RotateToLook(Transform lookAt, float time, Action onEnd, bool centerInternal = true);
        public void StopRotating();

        public void MoveTo(Transform point, float time, AnimationCurve curve, Action callback);
        public void MoveTo(HelicopterMoveToData moveToData);
        public void PauseMovement();
        public bool ResumeMovement();

        /// <summary>
        /// Way to animate while standing. Slowly moves locally in XY plane. Replaces Animating routine
        /// </summary>
        /// <param name="lookAt">If NULL will be ignored, if not will rotate and maintain lookAt rotation</param>
        void Loiter();
        void StopLoiter(bool moveBackToLocal = true);

    }
}