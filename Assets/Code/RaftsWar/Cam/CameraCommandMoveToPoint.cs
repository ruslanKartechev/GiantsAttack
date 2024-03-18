using System;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Cam
{
    public class CameraCommandMoveToPoint: ICommand<IPlayerCamera>
    {
        private Transform _point;
        private float _time;
        public CameraCommandMoveToPoint(Transform point, float time)
        {
            _point = point;
            _time = time;
        }

        public void Execute(IPlayerCamera target, Action onCompleted)
        {
            target.MoveToPoint(_point, _time, onCompleted);
        }
    }
}