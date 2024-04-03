using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class AnimatedVehicle : MonoBehaviour
    {
        [SerializeField] private bool _doMove;
        [SerializeField] private ExplodingVehicle _explodingVehicle;
        [SerializeField] private SimpleForwardMover _mover;

        private void Start()
        {
            if(_doMove)
                _mover.Move(() => {});
        }

        public void PushBack(Vector3 fromPoint, float force)
        {
            _mover.Stop();
            _explodingVehicle.Explode((_mover.Movable.position - fromPoint).normalized * force);
        }
    }
}