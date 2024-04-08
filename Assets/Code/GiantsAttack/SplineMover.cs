using System;
using System.Collections;
using SleepDev;
using UnityEngine;
using UnityEngine.Splines;

namespace GiantsAttack
{
    public class SplineMover : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _acceleration;
        [SerializeField] private SplineContainer _spline;
        public bool doDebug;
        
        private float _pathLength;
        private Coroutine _speedChanging;
        private Coroutine _moving;
        private float _interpolateT = 0f;
        private float _currentSpeed = 0f;
        
        public SplineContainer spline
        {
            get => _spline;
            set => _spline = value;
        }

        public float InterpolationT
        {
            get => _interpolateT;
            set => _interpolateT = value;
        }

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }
        
        public float acceleration
        {
            get => _acceleration;
            set => _acceleration = value;
        }

        #region EDITOR
#if UNITY_EDITOR
        public bool e_doControlPosition;
        [Range(0f,1f)] public float e_interpolationT = 0;
        
        public void E_AlignToInterpolateT()
        {
            if(e_doControlPosition)
                SetToT(e_interpolationT);
        }

        public void E_AddTransformToCurrentPosition()
        {
            var tr = new GameObject("spline_point").transform;
            tr.position = transform.position;
            tr.rotation = transform.rotation;
            tr.parent = transform.parent;
        }
#endif
        #endregion

        private void Start()
        {
            _pathLength = _spline.Spline.GetLength();
        }

        public void MoveNow()
        {
          Stop();
          _currentSpeed = _speed;
          _moving = StartCoroutine(Moving());
        }
        
        public void MoveAccelerated()
        {
            Stop();
            _currentSpeed = 0f;
            var time = Mathf.Abs(_currentSpeed - _speed) / _acceleration;
            _speedChanging = StartCoroutine(SpeedChanging(_currentSpeed, _speed, time));
            _moving = StartCoroutine(Moving());
        }

        public void ChangeSpeed(float finalSpeed, float duration)
        {
            if(_speedChanging != null)
                StopCoroutine(_speedChanging);
            _speedChanging = StartCoroutine(SpeedChanging(_currentSpeed, finalSpeed, duration));
        }
        
        public void Stop()
        {
            if(_moving != null)
                StopCoroutine(_moving);
            if(_speedChanging != null)
                StopCoroutine(_speedChanging);
        }

        public void SetToStart()
        {
            SetToT(0f);
        }

        public void SetToEnd()
        {
            SetToT(1f);
        }

        public void SetToT(float t)
        {
            _spline.Spline.Evaluate(t, out var pos, out var tangent, out var up);
            transform.position = spline.transform.TransformPoint(pos);
            transform.rotation = Quaternion.LookRotation(transform.parent.TransformVector(tangent), up);
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                UnityEditor.EditorUtility.SetDirty(transform);
#endif
        }

        private IEnumerator Moving()
        {
            if (doDebug)
            {
                CLog.LogGreen($"STARTED MOVING ________");
                // Debug.Break();
            }
            var spline = _spline.Spline;
            var totalLength = _pathLength;
            var passedLength = totalLength * _interpolateT;
            passedLength += Time.deltaTime * _speed;
            var tr = transform;
            while (_interpolateT <= 1f)
            {
                spline.Evaluate(_interpolateT, out var pos, out var tangent, out var up);
                tr.localPosition = pos;
                var rot = Quaternion.LookRotation(tangent, up);
                // tr.rotation = Quaternion.Lerp(tr.rotation, rot, .25f);
                tr.rotation = rot;
                if (doDebug)
                {
                    var fromPos = transform.position + Vector3.up * 20;
                    Debug.DrawLine(fromPos, fromPos + ((Vector3)tangent).normalized * 10, Color.red, 5f);
                }
                passedLength += Time.deltaTime * _speed;
                _interpolateT = passedLength / totalLength;
                yield return null;
            }
        }
        
        
        private IEnumerator SpeedChanging(float from, float to, float time)
        {
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                _speed = Mathf.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            _speed = to;
        }
    }
}