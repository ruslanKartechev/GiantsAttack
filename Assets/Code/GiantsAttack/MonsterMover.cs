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
        private Transform _moveToTarget;
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

        public void MoveTo(Transform target, float time, Action callback)
        {
            StopMovement();
            _moveToTarget = target;
            _animator.SetTrigger(_walkTriggerKey);
            _moving = StartCoroutine(Moving(time, callback));
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
            var elapsed = 0f;
            var t = 0f;
            var time = Quaternion.Angle(rotation, _movable.rotation) / rotationSpeed;
            var r1 = _movable.rotation;
            while (t <= 1f)
            {
                _movable.rotation = Quaternion.Lerp(r1, rotation, t);
                t = elapsed / time;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _movable.rotation = rotation;
        }
        
        private IEnumerator Moving(float time, Action callback)
        {
            var lookAtRotation = Quaternion.LookRotation(_moveToTarget.position - _movable.position);
            yield return RotatingTo(lookAtRotation, _rotationSpeed);
            var elapsed = 0f;
            var t = 0f;
            var p1 = _movable.position;
            while (t <= 1f)
            {
                _movable.position = Vector3.Lerp(p1, _moveToTarget.position, t);
                t = elapsed / time;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _movable.position = _moveToTarget.position;
            yield return RotatingTo(_moveToTarget.rotation, _rotationSpeed);
            callback?.Invoke();
        }
    }
}