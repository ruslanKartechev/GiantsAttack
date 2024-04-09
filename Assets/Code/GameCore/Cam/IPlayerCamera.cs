using System;
using SleepDev;
using UnityEngine;

namespace GameCore.Cam
{
    public interface IPlayerCamera
    {
        void Parent(Transform parent);
        void AddCommand(ICommand<IPlayerCamera> command);
        void SetPoint(Transform point);
        void SetRotation(Transform point);
        void SetPosition(Transform point);
        void MoveToPoint(Transform point, float time, Action onEnd);
        void MoveToPointToFollow(Transform point, float time, Action callback);
        void MoveToPointToParent(Transform point, float time, Action callback);
        
        void Wait(float time, Action onEnd);
        void StopMoving();
        void Shake();
        Transform Transform { get; }
    }
}