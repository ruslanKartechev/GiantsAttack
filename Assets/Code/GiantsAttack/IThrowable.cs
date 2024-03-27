using System;
using UnityEngine;

namespace GiantsAttack
{
    public interface IThrowable
    {
        Transform Transform { get;}
        void GrabBy(Transform hand, Action callback);
        void ThrowAt(Vector3 position, float time, Action flyEndCallback, Action<Collider> callbackHit);
        void Hide();
        void Explode();
    }
}