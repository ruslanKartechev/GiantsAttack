using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class VehicleMoveAndBreak : MonoBehaviour
    {
        [SerializeField] private bool _autoStart;
        [SerializeField] private float _moveTime;
        [SerializeField] private float _pushForceUp;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private ParticleSystem _explosion;
        [SerializeField] private ParticleSystem _fire;
        
        
        private Coroutine _working;
        
        private void Start()
        {
            if(_autoStart)
                Begin();
        }

        public void Begin()
        {
            Stop();
            _working = StartCoroutine(Working());
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private IEnumerator Working()
        {
            var elapsed = 0f;
            var time = _moveTime;
            var p1 = _movable.position;
            var p2 = _endPoint.position;
            var t = 0f;
            while (t <= 1f)
            {
                _movable.position = Vector3.Lerp(p1, p2, t);
                elapsed += Time.deltaTime * _curve.Evaluate(t);
                t = elapsed / time;
                yield return null;
            }
            _movable.position = p2;
            _explosion.Play();
            _rb.isKinematic = false;
            _collider.enabled = true;
            _rb.AddForce(Vector3.up * _pushForceUp, ForceMode.VelocityChange);
            _fire.gameObject.SetActive(true);
            _fire.Play();
        }
    }
}