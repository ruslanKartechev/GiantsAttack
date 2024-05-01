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
        public void RotateToLook(Transform lookAtlookAtPoint, float time, Action onEnd, bool keepLooking, bool centerInternal = true);
        public void RotateToLook(Vector3 lookAtPosition, float time, Action onEnd, bool centerInternal = true);
        
        public void StopRotating();

        public void MoveTo(Transform point, float time, AnimationCurve curve, Action callback);
        public void MoveTo(HelicopterMoveToData moveToData);
        public void ParentAndMoveLocal(Transform point, float time, AnimationCurve curve, Action callback);
        /// <summary>
        /// Resume movement on previous HelicopterMoveToData
        /// </summary>
        /// <returns> True if possible, false is not (no HelicopterMoveToData cashed) </returns>
        bool ResumeMovement();
        void PauseMovement();

        void BeginMovingAround(HelicopterMoveAroundData moveAroundData);
        void ChangeMovingAroundNode(HelicopterMoveAroundNode node);
        void StopMovingAround();
        
        /// <summary>
        /// Way to animate while standing. Slowly moves locally in XY plane. Replaces Animating routine
        /// </summary>
        /// <param name="lookAt">If NULL will be ignored, if not will rotate and maintain lookAt rotation</param>
        void Loiter();
        void StopLoiter(bool moveBackToLocal = true);
        void CenterInternal(float time = .5f);

    }
}