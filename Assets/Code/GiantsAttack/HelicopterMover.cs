using System;
using System.Collections;
using System.Collections.Generic;
using SleepDev;
using TMPro;
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

        public void BeginMovingOnCircle(CircularPath path, Transform lookAt, MoveOnCircleArgs args)
        {
            CLog.Log($"[HeliMover] Begin moving on circle");
            StopMovement();
            _moving = StartCoroutine(MovingOnCircle(path, lookAt, args));
        }
        
        public void StopMovement()
        {
            if(_moving != null)
                StopCoroutine(_moving);
        }

        public void BeginAnimatingVertically()
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

        public void RotateToLook(Transform lookAt, float time, Action onEnd, bool centerInternal = true)
        {
            StopRotating();
            var endRotation = Quaternion.LookRotation(lookAt.position - transform.position);
            _rotating = StartCoroutine(RotatingTo(endRotation, time, onEnd, centerInternal));
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
        
        
        #region Loitering
        public void Loiter()
        {
            StopAnimating();
            _animating = StartCoroutine(Loitering());
        }

        public void StopLoiter(bool moveBackToLocal = true)
        {
            const float returnTime = .5f;
            StopAnimating();
            if(moveBackToLocal)
                _animating = StartCoroutine(MovingLocal(Vector3.zero, returnTime));
        }
        
        private IEnumerator Loitering()
        {
            while (true)
            {
                var localPos = (Vector3)(UnityEngine.Random.insideUnitCircle * _animSettings.loiteringMagn);
                var time = (localPos - _internal.localPosition).magnitude / _animSettings.loiteringMoveSpeed;
                yield return MovingLocal(localPos, time);
            }
        }
        #endregion


        #region Evasion
        public void Evade(EDirection2D direction, Action callback, float evasionDistance)
        {
            StopAll();
            var evadeToPoint = transform.position;
            var angles = transform.eulerAngles;
            var dist = evasionDistance;
            if (dist == default)
                dist = evasionSettings.evadeDistance;
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
            yield return ChangingRotationUnscaledTime(transform, toTime, r1, r2);
            yield return ChangingRotationUnscaledTime(transform, fromTime, r2, r1);
        }
        #endregion

        private IEnumerator ChangingRotationUnscaledTime(Transform tr, float time, Quaternion r1, Quaternion r2)
        {
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

        private IEnumerator RotatingTo(Quaternion endRotation, float time, Action onEnd, bool centerInternal)
        {
            if (centerInternal)
            {
                const float internalRotTime = .3f;
                yield return ChangingRotationInternal(_internal.localRotation, Quaternion.identity, internalRotTime);
            }
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var tr = transform;
            var rot1 = tr.rotation;
            var rot2 = endRotation;
            while (t <= 1f)
            {
                tr.rotation = Quaternion.Lerp(rot1, rot2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            tr.rotation = rot2;
            onEnd?.Invoke();
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
        
        private IEnumerator ChangingRotationInternal(Quaternion localR1, Quaternion localR2, float time)
        {
            var elapsed = 0f;
            while (elapsed <= time)
            {
                _internal.localRotation = Quaternion.Lerp(localR1, localR2, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _internal.localRotation = localR2;
        }        
        
        private IEnumerator ChangingRotation(Transform target, Quaternion r1, Quaternion r2, float time)
        {
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
        
        private IEnumerator ChangingPosition(Transform target, Vector3 endPos, float time)
        {
            var p1 = target.position;
            var elapsed = 0f;
            while (elapsed <= time)
            {
                target.position = Vector3.Lerp(p1, endPos, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            target.position = endPos;
        }

        private IEnumerator MovingOnCircle(CircularPath path, Transform lookAt, MoveOnCircleArgs args)
        {
            var startAngle = path.startAngle;
            var vec = (_movable.position - path.root.position).XZPlane();
            
            if (args.refreshAngleOnStart == false)
            {
                startAngle = Vector3.Angle(path.root.forward, vec);
                // CLog.LogRed($"Angle calculated: {startAngle}");    
            }
            var startPos =  path.GetCirclePosAtAngle(startAngle);
            var awaitCoroutins = new List<Coroutine>();
            var lookAtRot = Quaternion.LookRotation(lookAt.position - startPos);
            var rotationTime = Quaternion.Angle(lookAtRot, _movable.rotation) / args.rotateToStartAngleSpeed;
            // CLog.LogYellow($"Rotation time: {rotationTime}");
            awaitCoroutins.Add(StartCoroutine(ChangingRotation(_movable,_movable.rotation, lookAtRot, rotationTime)));
            if (args.moveToStartPoint)
            {
                var startPosSetupTime = (_movable.position - startPos).magnitude / args.moveToStartPointSpeed;
                // CLog.LogYellow($"MovementTime time: {startPosSetupTime}");
                if (startPosSetupTime > .01)
                {
                    var movingToStart = StartCoroutine(ChangingPosition(_movable, startPos, startPosSetupTime));
                    awaitCoroutins.Add(movingToStart);       
                }
            }
            else
                _movable.position = startPos;
            foreach (var cor in awaitCoroutins)
                yield return cor;
            var angle = startAngle;
            while (true)
            {
                _movable.position = path.GetCirclePosAtAngle(angle);
                var rot = Quaternion.LookRotation(lookAt.position - _movable.position);
                _movable.rotation = rot;
                angle += Time.deltaTime * args.angleMoveSpeed;
                yield return null;
            }
        }

        private IEnumerator MovingOnCircle3(CircularPath path, Transform lookAtTarget, bool loop, Action callback)
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

#if UNITY_EDITOR
        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.W))
                Evade(EDirection2D.Up, () => {}, 0);
            if(Input.GetKeyDown(KeyCode.S))
                Evade(EDirection2D.Down, () => {}, 0);
            if(Input.GetKeyDown(KeyCode.D))
                Evade(EDirection2D.Right, () => {}, 0);
            if(Input.GetKeyDown(KeyCode.A))
                Evade(EDirection2D.Left, () => {}, 0);

        }
#endif
    }
}