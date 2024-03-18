using System;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Cam
{
    public class CameraCommandMoveToPointLocal: ICommand<IPlayerCamera>
    {
        private Transform _parent;
        private Transform _point;
        private float _time;
        
        public CameraCommandMoveToPointLocal(Transform parent, Transform point, float time)
        {
            _parent = parent;
            _point = point;
            _time = time;
        }

        public void Execute(IPlayerCamera target, Action onCompleted)
        {
            target.MoveToPointLocal(_parent, _point, _time, onCompleted);
        }
    }
}