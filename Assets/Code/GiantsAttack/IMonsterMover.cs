using System;
using UnityEngine;

namespace GiantsAttack
{
    public interface IMonsterMover
    {
        float MoveAnimationSpeed { get; set; }
        
        // Will rotate to look at the target and maintain Look rotation
        void RotateToLookAt(Transform target, float time, Action callback);
        void StopLookAt();

        // Will move to the point
        void MoveToPoint(Transform target, float time, Action callback);
        void MoveToPointSimRotation(Transform target, float time, Action callback);
        void StopMovement();
    }
}