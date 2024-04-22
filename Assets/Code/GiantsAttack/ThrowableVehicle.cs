using System;
using System.Collections.Generic;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class ThrowableVehicle : SimpleThrowable
    {
        [SerializeField] private VehicleTrail _trail;
        [SerializeField] private List<ParticleSystem> _particles;

        public override void GrabBy(Transform hand, Action callback)
        {
            foreach (var pp in _particles)
                pp.gameObject.SetActive(false);
            _trail.Off();
            _explosiveVehicle.ExplosionAndFire.PlayFire();
            base.GrabBy(hand,callback);
        }
    }
}