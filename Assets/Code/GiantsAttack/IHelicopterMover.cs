using System;
using UnityEngine;

namespace GiantsAttack
{
    public interface IHelicopterMover
    {
        MoverSettings Settings { get; set; }
        void SetPath(CircularPath path, Transform lookAtTarget);
        void BeginMovingOnCircle(CircularPath path, Transform lookAtTarget, bool loop, Action callback);
        
        void StopMovement();
        void BeginAnimating();
        void StopAnimating();

    }
}