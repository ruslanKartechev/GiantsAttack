using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class StartSequenceAnimation : LevelStartSequence
    {
        [SerializeField] private string _animationKey;
        [SerializeField] private Vector2 _explosionForce;
        [SerializeField] private Vector3 _explosionVector;
        [SerializeField] private List<ExplosiveVehicle> _explodingVehicles;
        [SerializeField] private List<StageListener> _listeners;

#if UNITY_EDITOR
        public override void E_Init()
        {
        }
#endif

        public override void Begin(Action onEnd)
        {
            Enemy.Animate(_animationKey, false);
            foreach (var listener in _listeners)
                listener.OnActivated();
            foreach (var vec in _explodingVehicles)
                vec.Explode(_explosionVector * _explosionForce.RandomInVec());
            onEnd.Invoke();
        }
    }
}