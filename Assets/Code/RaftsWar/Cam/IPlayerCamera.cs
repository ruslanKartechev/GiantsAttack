using System;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Cam
{
    public interface IPlayerCamera
    {
        void AddCommand(ICommand<IPlayerCamera> command);
        void SetPoint(Transform point);
        void SetRotation(Transform point);
        void SetPosition(Transform point);
        void MoveToPoint(Transform point, float time, Action onEnd);
        void MoveToPointLocal(Transform parent, Transform point, float time, Action callback);
        void Wait(float time, Action onEnd);
        void StopMoving();
        void Shake();
    }
}