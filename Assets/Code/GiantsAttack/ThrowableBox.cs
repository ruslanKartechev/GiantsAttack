using System;
using System.Collections;
using UnityEngine;
using SleepDev;

namespace GiantsAttack
{
    public class ThrowableBox : MonoBehaviour, IThrowable
    {
        [SerializeField] private float _moveTimeOnGrab = .2f;
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private Vector3 _localGrabbedPos;
        [SerializeField] private Vector3 _localGrabbedEulers;
        [SerializeField] private HitTriggerReceiver _hitTriggerReceiver;
        
        private Coroutine _moving;
        private Action<Collider> _hitCallback;

        public Transform Transform => transform;

        private void Awake()
        {
            _hitTriggerReceiver.Collider.enabled = false;
        }
        

        public void GrabBy(Transform hand, Action callback)
        {
            _hitTriggerReceiver.Collider.enabled = false;
            transform.parent = hand;
            _hitTriggerReceiver.enabled = false;
            if(_moving != null)
                StopCoroutine(_moving);
            _moving = StartCoroutine(MovingToGrabPos(callback));
        }

        public void ThrowAt(Vector3 position, float time, Action flyEndCallback, Action<Collider> callbackHit)
        {
            _hitTriggerReceiver.Callback = _hitCallback;
            _hitTriggerReceiver.Collider.enabled = true;
            _hitTriggerReceiver.enabled = true;
            transform.parent = null;
            _hitCallback = callbackHit;
            if(_moving != null)
                StopCoroutine(_moving);
            _moving = StartCoroutine(Flying(position, time, flyEndCallback));
        }

        public void Hide()
        {
            _hitTriggerReceiver.Collider.enabled = false;
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            _hitCallback.Invoke(other);            
        }

        private IEnumerator Flying(Vector3 endPos, float time, Action onEnd)
        {
            var startPos = transform.position;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            onEnd.Invoke();
        }
        
        private IEnumerator MovingToGrabPos(Action callback)
        {
            var elapsed = 0f;
            var time = _moveTimeOnGrab;
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

        
        #if UNITY_EDITOR
        [ContextMenu("E_SaveLocalPosRot")]
        public void E_SaveLocalPosRot()
        {
            _localGrabbedPos = transform.localPosition;
            _localGrabbedEulers = transform.localEulerAngles;
            UnityEditor.EditorUtility.SetDirty(this);

        }
        #endif
    }

}