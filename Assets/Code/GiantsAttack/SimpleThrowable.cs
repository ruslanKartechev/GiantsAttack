using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SimpleThrowable : MonoBehaviour, IThrowable
    {
        [SerializeField] protected float _rotationSpeed;
        [SerializeField] protected Vector3 _torqueVector;
        [SerializeField] protected ExplosiveVehicle _explosiveVehicle;
        private Coroutine _moving;
        private Action<Collider> _hitCallback;

        public Transform Transform => transform;


        public virtual void GrabBy(Transform hand, Action callback)
        {
            transform.parent = hand;
            if(_moving != null)
                StopCoroutine(_moving);
            callback?.Invoke();
        }

        public void FlyTo(Transform point, float time, Action flyEndCallback, Action<Collider> callbackHit)
        {
            transform.parent = null;
            _hitCallback = callbackHit;
            if(_moving != null)
                StopCoroutine(_moving);
            _moving = StartCoroutine(Flying(point, time, flyEndCallback));
        }

        public void TossTo(Vector3 force)
        {
            transform.parent = null;
            _explosiveVehicle.Coll.enabled = true;
            var torque = Vector3.Cross(force, Vector3.up);
            _explosiveVehicle.Rb.isKinematic = false;
            _explosiveVehicle.Rb.AddForce(force, ForceMode.Impulse);
            _explosiveVehicle.Rb.AddTorque(torque, ForceMode.Impulse);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Explode()
        {
            _explosiveVehicle.Explode();
            Hide();
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
            var rotVec = transform.forward * _torqueVector.z +
                         transform.right * _torqueVector.x +
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
        
    }
}