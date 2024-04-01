using System;
using UnityEngine;

namespace GiantsAttack
{
    public interface IThrowable
    {
        Transform Transform { get;}
        void GrabBy(Transform hand, Action callback);
        void ThrowAt(Transform point, float time, Action flyEndCallback, Action<Collider> callbackHit);
        void Hide();
        void Explode();
        void SetColliderActive(bool on);
    }
}