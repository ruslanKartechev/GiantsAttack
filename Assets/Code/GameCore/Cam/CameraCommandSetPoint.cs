using System;
using SleepDev;
using UnityEngine;

namespace GameCore.Cam
{
    public class CameraCommandSetPoint : ICommand<IPlayerCamera>
    {
        private Transform _point;
        
        public CameraCommandSetPoint(Transform point)
        {
            _point = point;
        }
        
        public void Execute(IPlayerCamera target, Action onCompleted)
        {
            target.SetPoint(_point);
            onCompleted?.Invoke();
        }
    }
}