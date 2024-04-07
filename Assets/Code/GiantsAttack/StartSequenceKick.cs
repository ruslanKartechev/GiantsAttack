﻿using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class StartSequenceKick : LevelStartSequence
    {
        [SerializeField] private string _animationKey;
        [SerializeField] private MonsterController _enemy;
        [SerializeField] private float _force;
        [SerializeField] private Transform _pushDirection;
        [SerializeField] private ExplodingVehicle _target;
        private Action _callback;

#if UNITY_EDITOR
        public override void E_Init()
        {
            if (_enemy == null)
            {
                _enemy = FindObjectOfType<MonsterController>();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        public override void Begin(Action onEnd)
        {
            _callback = onEnd;
            _enemy.Animate(_animationKey, false);
            _enemy.AnimEventReceiver.OnPunch += OnPunch;
        }

        private void OnPunch()
        {
            _enemy.AnimEventReceiver.OnPunch -= OnPunch;
            _target.Explode(_pushDirection.forward * _force);
            _callback.Invoke();
        }
    }
}