using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterMover : MonoBehaviour, IMonsterMover
    {
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _accelerationTime;
        [SerializeField] private float _animTransitionWaitTime = .1f;
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _rotatable;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _walkTriggerKey;
        private float _animationSpeed;

        private Coroutine _moving;
        private Coroutine _rotating;
        private Transform _targetPoint;
        private Transform _lookAtTarget;
        
        private static readonly int HashMoveSpeed = Animator.StringToHash("MoveSpeed");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int WalkToIdle = Animator.StringToHash("WalkToIdle");

        public float MoveAnimationSpeed { get; set; } = 1f;
        
        public void SetMoveAnimationSpeed(float val)
        {
            _animationSpeed = val;
            _animator.SetFloat(HashMoveSpeed, _animationSpeed);
        }

        public void PlayMoveAnimation()
        {
            SetMoveAnimationSpeed(MoveAnimationSpeed);
            _animator.SetTrigger(Walk);
        }

        public void RotateToLookAt(Transform target, float time, Action callback)
        {
            if (time == 0) return;
            _lookAtTarget = target;
            StopLookAt();
            _rotating = StartCoroutine(RotatingToLookAt(time, callback));
        }

        public void StopLookAt()
        {
            if(_rotating != null)
                StopCoroutine(_rotating);
        }

        public void MoveToPoint(Transform target, float time, Action callback)
        {
            if (time == 0) return;
            StopMovement();
            StopLookAt();
            _targetPoint = target;
            SetMoveAnimationSpeed(MoveAnimationSpeed);
            _moving = StartCoroutine(MovingToTargetPoint(time, callback));
        }
        
        public void MoveToPointSimRotation(Transform target, float time, Action callback)
        {
            if (time == 0) return;
            StopMovement();
            StopLookAt();
            _targetPoint = target;
            SetMoveAnimationSpeed(MoveAnimationSpeed);
            _animator.SetTrigger(_walkTriggerKey);
            _moving = StartCoroutine(MovingToTargetPointSimRotation(time, callback));
        }

        public void StopMovement()
        {
            _animator.ResetTrigger(_walkTriggerKey);
            if(_moving != null)
                StopCoroutine(_moving);
        }

        private IEnumerator RotatingToLookAt(float time, Action callback)
        {
            yield return null;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var rot1 = _rotatable.rotation;
            while (t <= 1f)
            {
                var vec = (_lookAtTarget.position - _rotatable.position).XZPlane();
                var rot2 = Quaternion.LookRotation(vec);
                _rotatable.rotation = Quaternion.Lerp(rot1, rot2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            callback?.Invoke();
            yield return LookingAt();
        }

        private IEnumerator LookingAt()
        {
            while (true)
            {
                var vec = (_lookAtTarget.position - _rotatable.position).XZPlane();
                _rotatable.rotation = Quaternion.LookRotation(vec);
                yield return null;
            }
        }

        private IEnumerator RotatingTo(Quaternion rotation, float rotationSpeed)
        {
            var elapsed = Time.deltaTime;
            var t = 0f;
            var r1 = _movable.rotation;
            var time = Quaternion.Angle(rotation, r1) / rotationSpeed;
            while (t < 1f)
            {
                _movable.rotation = Quaternion.Lerp(r1, rotation, t);
                t = elapsed / time;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _movable.rotation = rotation;
        }
        
        private IEnumerator RotatingToLerpAnimationSpeed(Quaternion rotation, float rotationSpeed, float finalAnimSpeed)
        {
            var elapsed = Time.deltaTime;
            var t = 0f;
            var r1 = _movable.rotation;
            var time = Quaternion.Angle(rotation, r1) / rotationSpeed;
            _animationSpeed = 0f;
            while (t < 1f)
            {
                _movable.rotation = Quaternion.Lerp(r1, rotation, t);
                SetMoveAnimationSpeed(Mathf.Lerp(0f, MoveAnimationSpeed * finalAnimSpeed, t));
                t = elapsed / time;
                elapsed += Time.deltaTime;
                yield return null;
            }
            SetMoveAnimationSpeed(MoveAnimationSpeed);
            _movable.rotation = rotation;
        }
        
        private IEnumerator MovingToTargetPointSimRotation(float time, Action callback)
        {
            var timeFactor = .5f;
            var lookAtRotation = Quaternion.LookRotation(_targetPoint.position - _movable.position);
            yield return RotatingToLerpAnimationSpeed(lookAtRotation, _rotationSpeed, MoveAnimationSpeed * timeFactor);
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var p1 = _movable.position;
            var accelerationTime = _accelerationTime;
            var r1 = _movable.rotation;
            var r2 = _targetPoint.rotation;
            while (t < 1f)
            {
                var pos = Vector3.Lerp(p1, _targetPoint.position, t);
                var rot = Quaternion.Lerp(r1, r2, t);
                _movable.SetXZPos(pos);
                _movable.rotation = rot;
                timeFactor = Mathf.Lerp(0f, 1f, elapsed / accelerationTime);
                SetMoveAnimationSpeed(timeFactor * MoveAnimationSpeed);
                elapsed += Time.deltaTime * timeFactor;
                yield return null;
            }
            _movable.SetXZPos(_targetPoint.position);
            _movable.rotation = _targetPoint.rotation;
            callback?.Invoke();
        }
        
        private IEnumerator MovingToTargetPoint(float time, Action callback)
        {
            var startTimeFactor = .4f;
            var timeFactor = startTimeFactor;
            var lookAtRotation = Quaternion.LookRotation((_targetPoint.position - _movable.position).XZPlane());
            yield return RotatingTo(lookAtRotation, _rotationSpeed);
            _animator.SetTrigger(_walkTriggerKey);
            yield return new WaitForSeconds(_animTransitionWaitTime * (1f / MoveAnimationSpeed));
            
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var p1 = _movable.position;
            var accelerationTime = _accelerationTime;
            while (t < 1f)
            {
                _movable.position = (Vector3.Lerp(p1, _targetPoint.position, t));
                t = elapsed / time;
                timeFactor = Mathf.Lerp(startTimeFactor, 1f, elapsed / accelerationTime);
                elapsed += Time.deltaTime * timeFactor;
                yield return null;
            }
            _movable.position = _targetPoint.position;
            // _movable.SetXZPos(_targetPoint.position);
            _animator.SetTrigger(WalkToIdle);
            yield return RotatingTo(_targetPoint.rotation, _rotationSpeed);
            callback?.Invoke();
        }
    }
}