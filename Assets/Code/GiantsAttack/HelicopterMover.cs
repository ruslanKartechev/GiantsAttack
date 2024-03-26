﻿using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterMover : MonoBehaviour, IHelicopterMover
    {
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _internal;
        [SerializeField] private HelicopterMoverSettingSo _settingSo;
        private HelicopterAnimSettings _animSettings;

        private float _t;
        private float _movingTime;
        private Coroutine _moving;
        private Coroutine _animating;
        private Coroutine _rotating;
        
        
        public HelicopterAnimSettings animSettings
        {
            get => _animSettings;
            set => _animSettings = value;
        }
        public MovementSettings movementSettings { get; set; }
        
        public EvasionSettings evasionSettings { get; set; }
        
        public MoverSettings Settings { get; set; }
        
        public Transform LookAt { get; set; }

        
        public void Awake()
        {
            evasionSettings = _settingSo.evasionSettings;
            movementSettings = _settingSo.movementSettings;
            animSettings = _settingSo.animSettingsSo.settings;
        }

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

        public void StopAll()
        {
            StopRotating();
            StopMovement();
            StopAnimating();
        }

        public void Evade(EDirection2D direction, Action callback)
        {
            StopMovement();
            StopAnimating();
            var evadeToPoint = transform.position;
            var angles = transform.eulerAngles;
            var dist = evasionSettings.evadeDistance;
            switch (direction)
            {
                case EDirection2D.Up:
                    evadeToPoint += Vector3.up * dist;
                    angles.x += evasionSettings.evadeAngles.x;
                    break;
                case EDirection2D.Down:
                    evadeToPoint -= Vector3.up * dist;
                    angles.x -= evasionSettings.evadeAngles.x;
                    break;
                case EDirection2D.Right:
                    evadeToPoint += transform.right * dist;
                    angles.z -= evasionSettings.evadeAngles.y;
                    break;
                case EDirection2D.Left:
                    evadeToPoint -= transform.right * dist;
                    angles.z += evasionSettings.evadeAngles.y;
                    break;
            }
            var time = evasionSettings.evadeTime;
            _moving = StartCoroutine(Evading(evadeToPoint, angles, time, callback));
            StartCoroutine(EvadeRotation( evasionSettings.rotToEvadeTimeFraction * time, 
                (1 - evasionSettings.rotToEvadeTimeFraction) * time, 
                angles));
        }

        public void RotateToLook(Transform lookAt, float time)
        {
            StopRotating();
            _rotating = StartCoroutine(RotatingToLook(lookAt, time));
        }

        public void StopRotating()
        {
            if(_rotating != null)
                StopCoroutine(_rotating);
        }

        public void MoveTo(Transform point, float time, AnimationCurve curve, Action callback)
        {
            StopMovement();
            if (curve == null)
                curve = movementSettings.defaultMoveCurve;
            _moving = StartCoroutine(MovingToPoint(point, time, curve, callback));
        }

        public void Loiter(Transform lookAt)
        {
            StopAnimating();
            _animating = StartCoroutine(Loitering(lookAt));
        }

        public void StopLoiter(bool moveBackToLocal = true)
        {
            StopAnimating();
            if(moveBackToLocal)
                _animating = StartCoroutine(MovingLocal(Vector3.zero, .5f));
        }

        private IEnumerator Evading(Vector3 endPoint, Vector3 angles, float time, Action callback)
        {
            var tr = transform;
            var p1 = tr.position;
            var p2 = endPoint;
            var e1 = transform.localEulerAngles;
            var e2 = angles;
            var elapsed = Time.unscaledDeltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                tr.position = Vector3.Lerp(p1, p2, t);
                if (t <= .5f)
                    tr.localEulerAngles = Vector3.Lerp(e1, e2, t * 2);
                else
                    tr.localEulerAngles = Vector3.Lerp(e2, e1, (t - .5f) * 2);
                elapsed += Time.unscaledDeltaTime;
                t = elapsed / time;
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

        private IEnumerator RotatingToLook(Transform lookAt, float time)
        {
            var elapsed = 0f;
            var t = elapsed / time;
            var tr = transform;
            var rot1 = tr.rotation;
            var rot2 = Quaternion.LookRotation(lookAt.position - tr.position);
            while (t <= 1f)
            {
                tr.rotation = Quaternion.Lerp(rot1, rot2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            tr.rotation = rot2;
        }

        private IEnumerator Loitering(Transform lookAt)
        {
            if (lookAt != null)
            {
                var rot1 = transform.rotation;
                var rot2 = Quaternion.LookRotation(lookAt.position - transform.position);
                var angle = Quaternion.Angle(rot1, rot2);
                yield return ChangingRotation(transform, rot1, rot2, angle / _animSettings.loiteringRotationSpeed);
            }
            while (true)
            {
                var localPos = (Vector3)(UnityEngine.Random.insideUnitCircle * _animSettings.loiteringMagn);
                var time = (localPos - _internal.localPosition).magnitude / _animSettings.loiteringMoveSpeed;
                if(lookAt != null)
                    yield return MovingLocalAndRotaing(localPos, time, lookAt);
                else
                    yield return MovingLocal(localPos, time);
            }
        }
        
        /// <summary>
        /// Moves "internal" to a given localPos. Also rotates "transform" itself to look at "lookAt"
        /// </summary>
        private IEnumerator MovingLocalAndRotaing(Vector3 localPos, float time, Transform lookAt)
        {
            var elapsed = 0f;
            var p1 = _internal.localPosition;
            while (elapsed <= time)
            {
                _internal.localPosition = Vector3.Lerp(p1, localPos, elapsed / time);
                transform.rotation = Quaternion.LookRotation(lookAt.position - transform.position);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _internal.localPosition = localPos;
        }
        
        private IEnumerator MovingLocal(Vector3 localPos, float time)
        {
            var elapsed = 0f;
            var p1 = _internal.localPosition;
            while (elapsed <= time)
            {
                _internal.localPosition = Vector3.Lerp(p1, localPos, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _internal.localPosition = localPos;
        }

        private IEnumerator ChangingRotation(Transform target, Quaternion r1, Quaternion r2, float time)
        {
            CLog.Log($"**** Changing rotation: time: {time}");
            var elapsed = 0f;
            while (elapsed <= time)
            {
                target.rotation = Quaternion.Lerp(r1, r2, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            target.rotation = r2;
        }

        
        /// <summary>
        /// Angles to "lean" when moving to next point
        /// </summary>
        private void CalculateLeanAngles(Transform endPoint, out Quaternion leanRot, out Quaternion endRot)
        {
            var tr = transform;
            var distVec = endPoint.position - tr.position;
            var projZ = Vector3.Dot(distVec, tr.forward);
            var projX = Vector3.Dot(distVec, tr.right);
            leanRot = transform.rotation;
            endRot = endPoint.rotation;
            var angles = new Vector3();
            if (projZ > 0)
                angles.x += movementSettings.leanAngles.x;
            else
                angles.x -= movementSettings.leanAngles.x;

            if (projX > 0)
                angles.z -= movementSettings.leanAngles.y;
            else
                angles.z += movementSettings.leanAngles.y;
            CLog.Log($"[HeliMover] LeanAngles {angles}");
            leanRot *= Quaternion.Euler(angles);
        }
        
        private IEnumerator MovingToPoint(Transform point, float time, AnimationCurve curve, Action callback)
        {
            var tr = transform;
            var p1 = tr.position;
            var r1 = tr.rotation;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            CalculateLeanAngles(point, out var leanRot, out var endRot);
            var leanRotT = movementSettings.leanRotT;
            while (t <= 1f)
            {
                tr.position = Vector3.Lerp(p1, point.position, t);
                if (t <= leanRotT)
                    tr.rotation = Quaternion.Lerp(r1, leanRot, t / leanRotT);
                else
                    tr.rotation = Quaternion.Lerp(leanRot, endRot, (t - leanRotT) / (1-leanRotT));
                
                elapsed += Time.deltaTime * curve.Evaluate(t);
                t = elapsed / time;
                yield return null;
            }
            tr.SetPositionAndRotation(point.position, point.rotation);
            callback?.Invoke();

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