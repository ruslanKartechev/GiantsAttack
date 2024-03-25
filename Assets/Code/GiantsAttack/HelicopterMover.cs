using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    
    public class HelicopterMover : MonoBehaviour, IHelicopterMover
    {
        [SerializeField] private float _evadeDistance;
        [SerializeField] private Vector2 _evandeAngles;
        [SerializeField] private float _evadeTime;
        [SerializeField, Range(0f,1f)] private float _rotToEvadeTimeFraction = .5f;

        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _internal;
        [SerializeField] private HelicopterAnimSettings _animSettings;

        private float _t;
        private float _movingTime;
        private Coroutine _moving;
        private Coroutine _animating;
        
        public HelicopterAnimSettings animSettings
        {
            get => _animSettings;
            set => _animSettings = value;
        }
        
        public MoverSettings Settings { get; set; }
        
        public Transform LookAt { get; set; }
  

        public void  BeginMovingOnCircle(CircularPath path, Transform lookAtTarget, bool loop, Action callback)
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

        public void Evade(EDirection2D direction, Action callback)
        {
            StopMovement();
            StopAnimating();
            var evadeToPoint = transform.position;
            var angles = transform.eulerAngles;
            switch (direction)
            {
                case EDirection2D.Up:
                    evadeToPoint += Vector3.up * _evadeDistance;
                    angles.x += _evandeAngles.x;
                    break;
                case EDirection2D.Down:
                    evadeToPoint -= Vector3.up * _evadeDistance;
                    angles.x -= _evandeAngles.x;
                    break;
                case EDirection2D.Right:
                    evadeToPoint += transform.right * _evadeDistance;
                    angles.z -= _evandeAngles.y;
                    break;
                case EDirection2D.Left:
                    evadeToPoint -= transform.right * _evadeDistance;
                    angles.z += _evandeAngles.y;
                    break;
            }
            _moving = StartCoroutine(Evading(evadeToPoint, angles, callback));
            StartCoroutine(EvadeRotation( _rotToEvadeTimeFraction * _evadeTime, 
                (1-_rotToEvadeTimeFraction) * _evadeTime, 
                angles));
        }

        private IEnumerator Evading(Vector3 endPoint, Vector3 angles, Action callback)
        {
            var tr = transform;
            var p1 = tr.position;
            var p2 = endPoint;
            var e1 = transform.localEulerAngles;
            var e2 = angles;
            var elapsed = Time.unscaledDeltaTime;
            var t = elapsed / _evadeTime;
            while (t <= 1f)
            {
                tr.position = Vector3.Lerp(p1, p2, t);
                if (t <= .5f)
                    tr.localEulerAngles = Vector3.Lerp(e1, e2, t * 2);
                else
                    tr.localEulerAngles = Vector3.Lerp(e2, e1, (t - .5f) * 2);
                elapsed += Time.unscaledDeltaTime;
                t = elapsed / _evadeTime;
                yield return null;
            }
            tr.position = p2;
            tr.localEulerAngles = e1;
            callback.Invoke();
        }

        private IEnumerator EvadeRotation(float toTime, float fromTime, Vector3 angles)
        {
            var r1 = transform.localRotation;
            var r2 = Quaternion.Euler(angles);
            yield return ChangingEulers(transform, toTime, r1, r2);
            yield return ChangingEulers(transform, fromTime, r2, r1);
        }

        private IEnumerator ChangingEulers(Transform tr, float time, Quaternion r1, Quaternion r2)
        {
            var e1 = transform.localEulerAngles;
            var elapsed = Time.unscaledDeltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                tr.localRotation = Quaternion.Lerp(r1, r2, t);
                elapsed += Time.unscaledDeltaTime;
                t = elapsed / time;
                yield return null;
            }
            tr.localRotation = r2;
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
            // do
            // {
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
            // } while (loop);
            
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

#if UNITY_EDITOR
        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.W))
                Evade(EDirection2D.Up, () => {});
            if(Input.GetKeyDown(KeyCode.S))
                Evade(EDirection2D.Down, () => {});
            if(Input.GetKeyDown(KeyCode.D))
                Evade(EDirection2D.Right, () => {});
            if(Input.GetKeyDown(KeyCode.A))
                Evade(EDirection2D.Left, () => {});

        }
#endif
    }
}