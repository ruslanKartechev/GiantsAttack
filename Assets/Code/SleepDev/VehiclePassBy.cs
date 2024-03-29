using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class VehiclePassBy : MonoBehaviour
    {
        [SerializeField] private bool _autoStart;
        [SerializeField] private float _moveTime;
        [SerializeField] private float _repeatDelay;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private Transform _startPoint;
        
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
            while (true)
            {
                _movable.gameObject.SetActive(true);
                _movable.position = _startPoint.position;
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
                _movable.gameObject.SetActive(false);
                yield return new WaitForSeconds(_repeatDelay);
            }
   
        }
    }
}