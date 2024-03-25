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

    }
}