using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterMover : MonoBehaviour, IHelicopterMover
    {
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _internal;
        [SerializeField] private HelicopterAnimSettings _animSettings;

        private float _t;
        private float _time;
        private CircularPath _path;
        private Coroutine _moving;
        private Coroutine _animating;
        
        public HelicopterAnimSettings animSettings
        {
            get => _animSettings;
            set => _animSettings = value;
        }
        public MoverSettings Settings { get; set; }
        public Transform LookAt { get; set; }
        
        public void SetPath(CircularPath path, Transform lookAtTarget)
        {
            _path = path;
            LookAt = lookAtTarget;
            _time = Mathf.Abs(path.endAngle - path.startAngle) / Settings.angularSpeed;
            CLog.LogRed($"Time set to {_time}");
        }

        public void BeginMovingOnCircle(CircularPath path, Transform lookAtTarget, bool loop, Action callback)
        {
            CLog.Log($"[HeliMover] Begin moving on circle");
            StopMovement();
            _moving = StartCoroutine(MovingOnCircle(path, lookAtTarget, loop, callback));
        }

        public void StopMovement()
        {
            if(_moving != null)
                StopCoroutine(_moving);
        }

        public void BeginAnimating()
        {
            StopAnimating();
            _animating = StartCoroutine(Animating());
        }

        public void StopAnimating()
        {
            if(_animating != null)
                StopCoroutine(_animating);
        }

        private IEnumerator ZeroInternalPosition()
        {
            const float timeToZero = .3f;
            var elapsed = 0f;
            var t = elapsed / timeToZero;
            var startY = _internal.localPosition.y;
            while (t <= 1f)
            {
                _internal.SetYLocalPos(Mathf.Lerp(startY,0f,t));
                elapsed += Time.deltaTime;
                t = elapsed / timeToZero;
                yield return null;
            }
            _internal.SetYLocalPos(0f);
        }
        
        private IEnumerator Animating()
        {
            if (_internal.localPosition.y != 0)
            {
                yield return ZeroInternalPosition();
            }
            while (true)
            {
                var elapsed = 0f;
                var t = elapsed / _animSettings.time;
                while (t <= 1f)
                {
                    var y = _animSettings.maxMagn * _animSettings.curve.Evaluate(t);
                    var pos = _internal.localPosition;
                    pos.y = y;
                    _internal.localPosition = pos;
                    elapsed += Time.deltaTime;
                    t = elapsed / _animSettings.time;
                    yield return null; 
                }
            }
        }

        private IEnumerator MovingOnCircle(CircularPath path, Transform lookAtTarget, bool loop, Action callback)
        {
            var time = Mathf.Abs(path.endAngle - path.startAngle) / Settings.angularSpeed;
            var elapsed = 0f;
            do
            {
                while (_t <= 1f)
                {
                    SetPos(_t);
                    elapsed += Time.deltaTime;
                    _t = elapsed / time;
                    yield return null;
                }
                _t = 1f;
                SetPos(_t);
                
                if (!loop)
                {
                    callback?.Invoke();
                    yield break;
                }
                
                while (_t >= 0)
                {
                    SetPos(_t);
                    elapsed -= Time.deltaTime;
                    _t = elapsed / time;
                    yield return null;
                }
                _t = 0;
            } while (loop);
            
            callback?.Invoke();
            
            void SetPos(float t)
            {
                var angle = Mathf.Lerp(path.startAngle, path.endAngle, t);
                var pos = path.GetCirclePosAtAngle(angle);
                _movable.position = pos;
                var rot = Quaternion.LookRotation(lookAtTarget.position - pos);
                _movable.rotation = rot;
            }
        }
    }
}