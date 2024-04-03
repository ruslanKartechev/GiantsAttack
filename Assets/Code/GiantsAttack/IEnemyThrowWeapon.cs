using System;
using UnityEngine;

namespace GiantsAttack
{
    public interface IEnemyThrowWeapon
    {
        GameObject GameObject { get; }
        IThrowable Throwable { get; }
        ITarget Target { get; }
        IHealth Health { get; }
        void MoveTo(Transform point, float time, Action onEnd);
        void AnimateMove(Action onEnd);
        void StopAnimate();
    }
}