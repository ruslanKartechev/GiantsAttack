using System;
using System.Collections;
using System.Collections.Generic;
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
        private HelicopterMoveToData _currentMoveToData;
        private HelicopterMoveAroundData _moveAroundData;

        private float _t;
        private float _movingTime;
        private Coroutine _moving;
        private Coroutine _subMoving;
        private Coroutine _animating;
        private Coroutine _rotating;
        
        
        public HelicopterAnimSettings animSettings
        {
            get => _animSettings;
            set => _animSettings = value;
        }
        public MovementSettings movementSettings { get; set; }
        
        public EvasionSettings evasionSettings { get; set; }
        
        // public MoverSettings Settings { get; set; }
        
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

        public void RotateToLook(Transform lookAtlookAtPoint, float time, Action onEnd, bool centerInternal = true)
        {
            StopRotating();
            _rotating = StartCoroutine(RotatingToLookAt(lookAtlookAtPoint, time, onEnd, centerInternal));
        }
        
        public void RotateToLook(Vector3 lookAtPosition, float time, Action onEnd, bool centerInternal = true)
        {
            CLog.LogRed($"********************* ROTATING TO LOOK AT {lookAtPosition}");
            StopRotating();
            _rotating = StartCoroutine(RotatingToLookAt(lookAtPosition, time, onEnd, centerInternal));
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

        public void MoveTo(HelicopterMoveToData moveToData)
        {
            _currentMoveToData = moveToData;
            StopMovement();
            if (_currentMoveToData.curve == null)
                _currentMoveToData.curve = movementSettings.defaultMoveCurve;
            if (_currentMoveToData.HasStarted ==false)
            {
                _currentMoveToData.HasStarted = true;
                _currentMoveToData.StartPos = _movable.position;
                _currentMoveToData.StartRot = _movable.rotation;
            }
            if(_currentMoveToData.lookAt == null)
                _moving = StartCoroutine(MovingToPoint(_currentMoveToData));
            else
                _moving = StartCoroutine(MovingWhileLookingAt(_currentMoveToData));
        }

        public void PauseMovement()
        {
            StopMovement(); 
        }

        public bool ResumeMovement()
        {
            CLog.LogBlue($"ResumeMovement");
            if (_currentMoveToData == null)
            {
                CLog.Log($"[{nameof(HelicopterMover)}] _currentMoveToData == null. Cannot resume");
                return false;
            }

            var d = (_currentMoveToData.endPoint.position - _currentMoveToData.StartPos).sqrMagnitude;
            var md = (_currentMoveToData.endPoint.position - _movable.position).sqrMagnitude;
            _currentMoveToData.time *= Mathf.Sqrt(md / d);
            _currentMoveToData.RefreshStartPosAndRot(_movable);
            MoveTo(_currentMoveToData);
            return true;
        }
        
        public void BeginMovingAround(HelicopterMoveAroundData moveAroundData)
        {
            StopMovement();
            StopRotating();
            _moveAroundData = moveAroundData;
            _moveAroundData.CalculateAngleHeightRadius(_movable);
            _moving = StartCoroutine(MovingAround());
        }

        public void ChangeMovingAroundNode(HelicopterMoveAroundNode node)
        {
            StopSubMovement();
            _subMoving = StartCoroutine(ChangingMoveAroundNode(node));
        }

        public void StopMovingAround()
        {
            StopMovement();
            StopSubMovement();
        }

        public void StopSubMovement()
        {
            if (_subMoving != null)
                StopCoroutine(_subMoving);
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
            var endPoint = transform.position;
            var angles = Vector3.zero;
            var dist = evasionDistance;
            if (dist == default)
                dist = evasionSettings.evadeDistance;
            switch (direction)
            {
                case EDirection2D.Up:
                    endPoint += Vector3.up * dist;
                    angles.x += evasionSettings.evadeAngles.x;
                    break;
                case EDirection2D.Down:
                    endPoint -= Vector3.up * dist;
                    angles.x -= evasionSettings.evadeAngles.x;
                    break;
                case EDirection2D.Right:
                    endPoint += transform.right * dist;
                    angles.z -= evasionSettings.evadeAngles.y;
                    break;
                case EDirection2D.Left:
                    endPoint -= transform.right * dist;
                    angles.z += evasionSettings.evadeAngles.y;
                    break;
            }
            var time = evasionSettings.evadeTime;
            _moving = StartCoroutine(Evading(endPoint, time, callback, angles));
        }

        private IEnumerator Evading(Vector3 endPoint, float time, Action callback, Vector3 angles)
        {
            _rotating = StartCoroutine(EvadeRotation( evasionSettings.rotToEvadeTimeFraction * time, 
                (1 - evasionSettings.rotToEvadeTimeFraction) * time, angles));
            _animating = StartCoroutine(EvadeMoving(endPoint, time));
            yield return _rotating;
            yield return _animating;
            callback.Invoke();
        }
        
        private IEnumerator EvadeMoving(Vector3 endPoint, float time)
        {
            var p1 = transform.position;
            var p2 = endPoint;
            var elapsed = Time.unscaledDeltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                transform.position = Vector3.Lerp(p1, p2, t);
                elapsed += Time.unscaledDeltaTime;
                t = elapsed / time;
                yield return null;
            }
            transform.position = p2;
        }

        private IEnumerator EvadeRotation(float toTime, float fromTime, Vector3 angles)
        {
            var tr = _movable;
            var r1 = tr.rotation;
            var r2 = r1 * Quaternion.Euler(angles);
            var elapsed = Time.unscaledDeltaTime;
            while (elapsed <= toTime)
            {
                tr.rotation = Quaternion.Lerp(r1, r2, elapsed / toTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            tr.rotation = r2;
            yield return null;
            elapsed = 0f;
            while (elapsed <= fromTime)
            {
                tr.rotation = Quaternion.Lerp(r2, r1, elapsed / fromTime);
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            tr.rotation = r1;
        }
        #endregion

        
        
        #region Rotation and local movement
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
        private IEnumerator RotatingToLookAt(Vector3 lookPosition, float time, Action onEnd, bool centerInternal)
        {
            if (centerInternal)
            {
                const float internalRotTime = .3f;
                yield return ChangingRotationInternal(_internal.localRotation, Quaternion.identity, internalRotTime);
            }
            var elapsed = Time.deltaTime;
            var tr = transform;
            var rot1 = tr.rotation;
            var rot2 = Quaternion.LookRotation(lookPosition - tr.position);
            while (elapsed < time)
            {
                tr.rotation = Quaternion.Lerp(rot1, rot2, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            tr.rotation = rot2;
            onEnd?.Invoke();
        }

        private IEnumerator RotatingToLookAt(Transform lookPoint, float time, Action onEnd, bool centerInternal)
        {
            if (centerInternal)
            {
                const float internalRotTime = .3f;
                yield return ChangingRotationInternal(_internal.localRotation, Quaternion.identity, internalRotTime);
            }
            var elapsed = Time.deltaTime;
            var tr = transform;
            var rot1 = tr.rotation;
            while (elapsed < time)
            {
                var rot2 = Quaternion.LookRotation(lookPoint.position - tr.position);
                tr.rotation = Quaternion.Lerp(rot1, rot2, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            tr.rotation = Quaternion.LookRotation(lookPoint.position - tr.position);
            onEnd?.Invoke();
        }
        
        private IEnumerator RotatingTo(Quaternion endRotation, float time, Action onEnd, bool centerInternal)
        {
            if (centerInternal)
            {
                const float internalRotTime = .3f;
                yield return ChangingRotationInternal(_internal.localRotation, Quaternion.identity, internalRotTime);
            }
            var elapsed = Time.deltaTime;
            var tr = transform;
            var rot1 = tr.rotation;
            var rot2 = endRotation;
            while (elapsed < time)
            {
                tr.rotation = Quaternion.Lerp(rot1, rot2, elapsed / time);
                elapsed += Time.deltaTime;
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
        #endregion

        private Quaternion GetLeanRotation(Transform endPoint)
        {
            var tr = transform;
            var distVec = endPoint.position - tr.position;
            var projZ = Vector3.Dot(distVec, tr.forward);
            var projX = Vector3.Dot(distVec, tr.right);
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
            return Quaternion.Euler(angles);
        }
        
        /// <summary>
        /// Angles to "lean" when moving to next point
        /// </summary>
        private void GetLeanAndFinalRotation(Transform endPoint, out Quaternion leanRot, out Quaternion endRot)
        {
            leanRot = transform.rotation;
            endRot = endPoint.rotation;
            leanRot *= GetLeanRotation(endPoint);
        }

        private IEnumerator MovingWhileLookingAt(HelicopterMoveToData moveToData)
        {
            var tr = transform;
            var p1 = moveToData.StartPos;
            var t = moveToData.LerpT;
            var time = moveToData.time;
            var elapsed = t * time;
            // var leanRot= GetLeanRotation(moveToData.endPoint);
            // var leanRotT = movementSettings.leanRotT;
            var resRot = tr.rotation;
            while (t <= 1f)
            {
                tr.position = Vector3.Lerp(p1, moveToData.endPoint.position, t);
                var targetRot = Quaternion.LookRotation(moveToData.lookAt.position - tr.position);
                targetRot = resRot = Quaternion.RotateTowards(tr.rotation, targetRot, movementSettings.rotationSpeed * Time.deltaTime);
                // if (t <= leanRotT)
                //     targetRot *= Quaternion.Lerp(Quaternion.identity, leanRot, t / leanRotT);
                // else
                //     targetRot *= Quaternion.Lerp(leanRot, Quaternion.identity, (t - leanRotT) / (1-leanRotT));
                tr.rotation = targetRot;
                elapsed += Time.deltaTime * moveToData.curve.Evaluate(t);
                moveToData.LerpT = t = elapsed / time;
                yield return null;
            }
            tr.position = moveToData.endPoint.position;
            // tr.SetPositionAndRotation(moveToData.endPoint.position, moveToData.endPoint.rotation);
            moveToData.callback?.Invoke();
        }        
        
        private IEnumerator MovingToPoint(HelicopterMoveToData moveToData)
        {
            var tr = transform;
            var p1 = moveToData.StartPos;
            var r1 = moveToData.StartRot;
            var t = moveToData.LerpT;
            var time = moveToData.time;
            var elapsed = t * time;
            GetLeanAndFinalRotation(moveToData.endPoint, out var leanRot, out var endRot);
            var leanRotT = movementSettings.leanRotT;
            while (t <= 1f)
            {
                tr.position = Vector3.Lerp(p1, moveToData.endPoint.position, t);
                if (t <= leanRotT)
                    tr.rotation = Quaternion.Lerp(r1, leanRot, t / leanRotT);
                else
                    tr.rotation = Quaternion.Lerp(leanRot, endRot, (t - leanRotT) / (1-leanRotT));
                
                elapsed += Time.deltaTime * moveToData.curve.Evaluate(t);
                moveToData.LerpT = t = elapsed / time;
                yield return null;
            }
            tr.SetPositionAndRotation(moveToData.endPoint.position, moveToData.endPoint.rotation);
            moveToData.callback?.Invoke();
        }
        
        private IEnumerator MovingToPoint(Transform point, float time, AnimationCurve curve, Action callback)
        {
            var tr = transform;
            var p1 = tr.position;
            var r1 = tr.rotation;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            GetLeanAndFinalRotation(point, out var leanRot, out var endRot);
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
                startAngle = Vector3.SignedAngle(path.root.forward, vec, path.root.up);
            var startPos =  path.GetCirclePosAtAngle(startAngle);
            var y = transform.position.y;
            startPos.y = y;
            
            var awaitCoroutins = new List<Coroutine>();
            var lookAtRot = Quaternion.LookRotation(lookAt.position - startPos);
            var rotationTime = Quaternion.Angle(lookAtRot, _movable.rotation) / args.rotateToStartAngleSpeed;
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
            var tiltAngle = args.maxTiltAngle;
            var tiltAngleMax = args.maxTiltAngle;
            var tiltAngleMin = args.minTiltAngle;
            var elapsed = 0f;
            var period = args.tiltPeriod;
            while (true)
            {
                var pos = path.GetCirclePosAtAngle(angle);
                pos.y = y;
                _movable.position = pos;
                var rot = Quaternion.LookRotation(lookAt.position - _movable.position);
                rot *= Quaternion.Euler(new Vector3(0f, 0f,  tiltAngle * Mathf.Sign(args.circleAngleSpeed)));
                angle += Time.deltaTime * args.circleAngleSpeed;
                
                tiltAngle = Mathf.Lerp(tiltAngleMin, tiltAngleMax, elapsed / period);
                elapsed += Time.deltaTime;
                if (elapsed >= period)
                {
                    elapsed = 0f;
                    (tiltAngleMax, tiltAngleMin) = (tiltAngleMin, tiltAngleMax);
                }
                _movable.rotation = Quaternion.Lerp(_movable.rotation, rot, args.angularSpeed);
                yield return null;
            }
        }

        #region MovingAround Coroutines
        private IEnumerator MovingAround()
        {
            while (true)
            {
                _moveAroundData.CalculatePositionAndRotation(out var pos, out var rot);
                _movable.SetPositionAndRotation(pos, rot);
                // CLog.Log($"Angle {_moveAroundData.angle}, Radius {_moveAroundData.radius}, Height {_moveAroundData.height}");
                yield return null;
            }
        }

        private IEnumerator ChangingMoveAroundNode(HelicopterMoveAroundNode node)
        {
            // CLog.LogRed($"ChangingMoveAroundNode, Angle {node.changeAngle}, Radius {node.changeRadius}");
            var awaitCors = new List<Coroutine>(3);
            if (node.changeAngle)
            {
                _moveAroundData.targetAngle = node.angle;
                _moveAroundData.timeToChangeAngle = node.timeToChangeAngle;
                _moveAroundData.elapsedAngleTime = 0f;
                awaitCors.Add(StartCoroutine(ChangingMoveAroundAngle()));
            }
            if (node.changeRadius)
            {
                _moveAroundData.targetRadius = node.radius;
                _moveAroundData.timeToChangeRadius = node.timeToChangeRadius;
                _moveAroundData.elapsedRadiusTime = 0f;
                awaitCors.Add(StartCoroutine(ChangingMoveAroundRadius()));
            }
            if (node.changeHeight)
            {
                _moveAroundData.targetHeight = node.height;
                _moveAroundData.timeToChangeHeight = node.timeToChangeHeight;
                _moveAroundData.elapsedHeightTime = 0f;
                awaitCors.Add(StartCoroutine(ChangingMoveAroundHeight()));
            }
            
            foreach (var cor in awaitCors)
                yield return cor;
            
        }

        private IEnumerator ChangingMoveAroundAngle()
        {
            _moveAroundData.angleCashed = _moveAroundData.angle;
            while (_moveAroundData.elapsedAngleTime < _moveAroundData.timeToChangeAngle)
            {
                _moveAroundData.angle = Mathf.Lerp(_moveAroundData.angleCashed, _moveAroundData.targetAngle,
                    _moveAroundData.elapsedAngleTime / _moveAroundData.timeToChangeAngle);
                _moveAroundData.elapsedAngleTime += Time.deltaTime;
                yield return null;
            }
            _moveAroundData.angle = _moveAroundData.targetAngle;
        }

        private IEnumerator ChangingMoveAroundRadius()
        {
            _moveAroundData.radiusCashed = _moveAroundData.radius;
            while (_moveAroundData.elapsedRadiusTime < _moveAroundData.timeToChangeRadius)
            {
                _moveAroundData.radius = Mathf.Lerp(_moveAroundData.radiusCashed, _moveAroundData.targetRadius,
                    _moveAroundData.elapsedRadiusTime / _moveAroundData.timeToChangeRadius);
                _moveAroundData.elapsedRadiusTime += Time.deltaTime;
                yield return null;
            }
            _moveAroundData.radius = _moveAroundData.targetRadius;
        }
        
        private IEnumerator ChangingMoveAroundHeight()
        {
            _moveAroundData.heightCashed = _moveAroundData.height;
            while (_moveAroundData.elapsedHeightTime < _moveAroundData.timeToChangeHeight)
            {
                _moveAroundData.height = Mathf.Lerp(_moveAroundData.heightCashed, _moveAroundData.targetHeight,
                    _moveAroundData.elapsedHeightTime / _moveAroundData.timeToChangeHeight);
                _moveAroundData.elapsedHeightTime += Time.deltaTime;
                yield return null;
            }
            _moveAroundData.height = _moveAroundData.targetHeight;
        }
        #endregion

        
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