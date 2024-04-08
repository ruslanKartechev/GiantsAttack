using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterMover : MonoBehaviour, IMonsterMover
    {
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _rotatable;
        [SerializeField] private Animator _animator;
        [SerializeField] private string _walkTriggerKey;

        private Coroutine _moving;
        private Coroutine _rotating;
        private Transform _targetPoint;
        private Transform _lookAtTarget;

        public void RotateToLookAt(Transform target, float time, Action callback)
        {
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
            StopMovement();
            StopLookAt();
            _targetPoint = target;
            _animator.SetTrigger(_walkTriggerKey);
            _moving = StartCoroutine(MovingToTargetPoint(time, callback));
        }
        
        public void MoveToPointSimRotation(Transform target, float time, Action callback)
        {
            StopMovement();
            StopLookAt();
            _targetPoint = target;
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
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var rot1 = _rotatable.rotation;
            while (t <= 1f)
            {
                var vec = (_lookAtTarget.position - _rotatable.position).XZPlane();
                var rot2 = Quaternion.LookRotation(vec);
                _rotatable.rotation =Quaternion.Lerp(rot1, rot2, t);
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
        
        private IEnumerator MovingToTargetPointSimRotation(float time, Action callback)
        {
            var lookAtRotation = Quaternion.LookRotation(_targetPoint.position - _movable.position);
            yield return RotatingTo(lookAtRotation, _rotationSpeed);
            var elapsed = 0f;
            var t = 0f;
            var p1 = _movable.position;
            var r1 = _movable.rotation;
            var r2 = _targetPoint.rotation;
            while (t < 1f)
            {
                var pos = Vector3.Lerp(p1, _targetPoint.position, t);
                var rot = Quaternion.Lerp(r1, r2, t);
                _movable.SetPositionAndRotation(pos, rot);
                t = elapsed / time;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _movable.SetPositionAndRotation(_targetPoint.position, _targetPoint.rotation);
            callback?.Invoke();
        }
        
        private IEnumerator MovingToTargetPoint(float time, Action callback)
        {
            var lookAtRotation = Quaternion.LookRotation(_targetPoint.position - _movable.position);
            yield return RotatingTo(lookAtRotation, _rotationSpeed);
            var elapsed = 0f;
            var t = 0f;
            var p1 = _movable.position;
            while (t < 1f)
            {
                _movable.position = Vector3.Lerp(p1, _targetPoint.position, t);
                t = elapsed / time;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _movable.position = _targetPoint.position;
            yield return RotatingTo(_targetPoint.rotation, _rotationSpeed);
            callback?.Invoke();
        }
    }
}