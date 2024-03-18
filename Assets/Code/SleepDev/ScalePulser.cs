using DG.Tweening;
using UnityEngine;

namespace SleepDev
{
    public class ScalePulser : MonoBehaviour
    {
        [SerializeField] private float _magnitude;
        [SerializeField] private float _time;
        [SerializeField] private bool _autoStart;
        [SerializeField] private Transform _target;
        private Sequence _sequence;
        
        public Transform Target => _target;

        private void OnEnable()
        {
            if (_autoStart == false)
                return;
            Begin();
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Begin()
        {
            var startScale = _target.localScale.x;
            Stop();
            _sequence = DOTween.Sequence();
            _target.localScale = Vector3.one * (startScale - _magnitude);
            _sequence.Append(_target.DOScale(Vector3.one * (startScale + _magnitude), _time));
            _sequence.Append(_target.DOScale(Vector3.one * (startScale - _magnitude), _time));
            _sequence.SetLoops(-1);
        }

        public void Stop()
        {
            _sequence?.Kill();
        }

        public void Reset()
        {
            Stop();
            _target.localScale = Vector3.one;
        }
    }
}