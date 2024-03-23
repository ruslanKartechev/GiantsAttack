using System;
using System.Collections;
using UnityEngine;

namespace GiantsAttack
{
    public class GrabTarget : MonoBehaviour, IGrabTarget
    {
        [SerializeField] private Vector3 _localGrabbedPos;
        [SerializeField] private Vector3 _localGrabbedEulers;
        [SerializeField] private float _moveTime = .2f;

        public Transform Transform => transform;
        public void GrabBy(Transform hand, Action callback)
        {
            transform.parent = hand;
            StartCoroutine(MovingToGrabPos(callback));
        }

        private IEnumerator MovingToGrabPos(Action callback)
        {
            var elapsed = 0f;
            var time = _moveTime;
            var t = elapsed / time;
            var p1 = transform.localPosition;
            var p2 = _localGrabbedPos;
            var r1 = transform.localRotation;
            var r2 = Quaternion.Euler(_localGrabbedEulers);
            while (t <= 1f)
            {
                transform.localPosition = Vector3.Lerp(p1, p2, t);
                transform.localRotation = Quaternion.Lerp(r1, r2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            transform.localPosition = p2;
            transform.localRotation = r2;
            callback?.Invoke();
        }
    }
}