using System;
using UnityEngine;

namespace GiantsAttack
{
    public interface IMonsterMover
    {
        // Will rotate to look at the target and maintain Look rotation
        void RotateToLookAt(Transform target, float time, Action callback);
        void StopLookAt();

        // Will move to the point
        void MoveTo(Transform target, float time, Action callback);
        void StopMovement();
    }
}