using System;
using System.Collections.Generic;
using SleepDev;
using SleepDev.Utils;
using UnityEngine;

namespace GiantsAttack
{
    public class StartAnimationSequence : LevelStartSequence
    {
        [SerializeField] private string _animationKey;
        [SerializeField] private MonsterController _enemy;
        [SerializeField] private Vector2 _explosionForce;
        [SerializeField] private Vector3 _explosionVector;
        [SerializeField] private List<ExplosiveVehicle> _explodingVehicles;

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
            _enemy.Animate(_animationKey, false);
            foreach (var vec in _explodingVehicles)
                vec.Explode(_explosionVector * _explosionForce.RandomInVec());
            onEnd.Invoke();
        }
    }
}