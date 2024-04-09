using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SimpleThrowable : MonoBehaviour, IThrowable
    {
        [SerializeField] protected bool _doRotateWhenGrabbed;
        [SerializeField] protected bool _doMoveToLocalPos;
        [SerializeField] protected float _moveTimeOnGrab = .2f;
        [SerializeField] protected float _rotationSpeed;
        [SerializeField] protected Vector3 _torqueVector;
        [SerializeField] protected Vector3 _localGrabbedPos;
        [SerializeField] protected Vector3 _localGrabbedEulers;
        [SerializeField] protected HitTriggerReceiver _hitTriggerReceiver;
        [SerializeField] protected ParticleSystem _explosionParticles;
        [SerializeField] protected Collider _collider;
        [SerializeField] protected Rigidbody _rb;

        private Coroutine _moving;
        private Action<Collider> _hitCallback;

        public Transform Transform => transform;

        protected virtual void Awake()
        {
            _hitTriggerReceiver.Collider.enabled = false;
        }
        

        public virtual void GrabBy(Transform hand, Action callback)
        {
            _hitTriggerReceiver.Collider.enabled = false;
            transform.parent = hand;
            _hitTriggerReceiver.enabled = false;
            if(_moving != null)
                StopCoroutine(_moving);
            if(_doMoveToLocalPos)
                _moving = StartCoroutine(MovingToGrabPos(callback));
            callback?.Invoke();
        }

        public void FlyTo(Transform point, float time, Action flyEndCallback, Action<Collider> callbackHit)
        {
            _hitTriggerReceiver.Callback = _hitCallback;
            _hitTriggerReceiver.Collider.enabled = true;
            _hitTriggerReceiver.enabled = true;
            transform.parent = null;
            _hitCallback = callbackHit;
            if(_moving != null)
                StopCoroutine(_moving);
            _moving = StartCoroutine(Flying(point, time, flyEndCallback));
        }

        public void TossTo(Vector3 force)
        {
            _collider.enabled = true;
            _collider.isTrigger = false;
            _rb.isKinematic = false;
            transform.parent = null;
            var torque = Vector3.Cross(force, Vector3.up);
            _rb.AddForce(force, ForceMode.Impulse);
            _rb.AddTorque(torque, ForceMode.Impulse);
        }

        public void Hide()
        {
            _hitTriggerReceiver.Collider.enabled = false;
            gameObject.SetActive(false);
        }

        public void Explode()
        {
            _explosionParticles.transform.parent = transform.parent;
            _explosionParticles.gameObject.SetActive(true);
            _explosionParticles.Play();
            Hide();
        }

        public void SetColliderActive(bool on)
        {
            _collider.enabled = on;
        }

        private void OnTriggerEnter(Collider other)
        {
            _hitCallback.Invoke(other);            
        }

        private IEnumerator Flying(Transform point, float time, Action onEnd)
        {
            var startPos = transform.position;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var rotVec = transform.forward * _torqueVector.z+
                         transform.right * _torqueVector.x+
                         transform.up * _torqueVector.y;
            // var endPos = point.position;
            while (t <= 1f)
            {
                transform.position = Vector3.Lerp(startPos, point.position, t);
                transform.Rotate(rotVec, _rotationSpeed * Time.deltaTime);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            onEnd.Invoke();
        }
        
        private IEnumerator MovingToGrabPos(Action callback)
        {
            var posChange = StartCoroutine(PositionChanging());
            if (_doRotateWhenGrabbed)
            {
                var rotChange = StartCoroutine(RotationChanging());
                yield return rotChange;
            }
            yield return posChange;
            callback?.Invoke();
        }

        private IEnumerator PositionChanging()
        {
            var elapsed = 0f;
            var time = _moveTimeOnGrab;
            var t = elapsed / time;
            var p1 = transform.localPosition;
            var p2 = _localGrabbedPos;
            while (t <= 1f)
            {
                transform.localPosition = Vector3.Lerp(p1, p2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            transform.localPosition = p2;
        }
        
        private IEnumerator RotationChanging()
        {
            var elapsed = 0f;
            var time = _moveTimeOnGrab;
            var t = elapsed / time;
            var r1 = transform.localRotation;
            var r2 = Quaternion.Euler(_localGrabbedEulers);
            while (t <= 1f)
            {
                transform.localRotation = Quaternion.Lerp(r1, r2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            transform.localRotation = r2;
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