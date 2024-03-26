using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterMover : MonoBehaviour, IMonsterMover
    {
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
            _moveToTarget = target;
            _animator.SetTrigger(_walkTriggerKey);
            StopMovement();
            _moving = StartCoroutine(Moving(time, callback));
        }

        public void StopMovement()
        {
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

        private IEnumerator Moving(float time, Action callback)
        {
            var elapsed = 0f;
            var t = 0f;
            var p1 = _movable.position;
            var r1 = _movable.rotation;
            while (t <= 1f)
            {
                _movable.position = Vector3.Lerp(p1, _moveToTarget.position, t);
                _movable.rotation = Quaternion.Lerp(r1, _moveToTarget.rotation, t);
                t = elapsed / time;
                elapsed += Time.deltaTime;
                yield return null;
            }
            _movable.position = _moveToTarget.position;
            _movable.rotation = _moveToTarget.rotation;
            callback?.Invoke();
        }
    }
}