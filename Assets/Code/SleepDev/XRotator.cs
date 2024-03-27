﻿using UnityEngine;

namespace SleepDev
{
    public class XRotator : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private Transform _target;

        private void Update()
        {
            var angles = _target.localEulerAngles;
            angles.x += _speed * Time.deltaTime;
            _target.localEulerAngles = angles;
        }
    }
}