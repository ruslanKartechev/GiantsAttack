using System;
using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class CircularHelicopterMover : MonoBehaviour
    {
        [SerializeField] private Transform _center;
        [SerializeField] private Transform _movable;
        [SerializeField] private float _angularSpeed;
        [SerializeField] private float _radius;
        [SerializeField] private float _tiltAngleZ;
        [SerializeField] private float _tiltAngleX;
        [SerializeField] private bool _autoStart;
        private Coroutine _working;
        
        private void Start()
        {
            if(_autoStart)
                Begin();
        }

        public void Begin()
        {
            Stop();
            _working = StartCoroutine(Working());
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        private IEnumerator Working()
        {
            var angle = 0f;
            var radVec = _center.forward * _radius;
            var center = _center.position;
            while (true)
            {
                var pos = center + Quaternion.Euler(0f, angle, 0f) * radVec;
                var forw = Vector3.Cross(pos - center, Vector3.up);
                var rot = Quaternion.LookRotation(forw) * Quaternion.Euler(_tiltAngleX, 0f, _tiltAngleZ);
                _movable.SetPositionAndRotation(pos, rot);
                angle += Time.deltaTime * _angularSpeed;
                yield return null;
            }
        }
    }
}