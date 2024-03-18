using DG.Tweening;
using UnityEngine;

namespace SleepDev.UIUtils
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeElement : MonoBehaviour
    {
        [SerializeField] private float _delay;
        [SerializeField] private float _durationIn;
        [SerializeField] private float _durationOut;
        [SerializeField] private Ease _ease;

        public float delay
        {
            get => _delay;
            set => _delay = value;
        }

        public float durationIn
        {
            get => _durationIn;
            set => _durationIn = value;
        }
        
        public float durationOut
        {
            get => _durationOut;
            set => _durationOut = value;
        }

        public Ease ease
        {
            get => _ease;
            set => _ease = value;
        }

        [SerializeField] private CanvasGroup _canvasGroup;

        

        private Tween _tween;
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
        }
#endif

        public void FadeIn()
        {
            _tween?.Kill();
            _canvasGroup.alpha = 0f;
            _tween = _canvasGroup.DOFade(1f, _durationIn).SetEase(_ease).SetDelay(_delay);
        }

        public void FadeOut()
        {
            _tween?.Kill();
            _canvasGroup.alpha = 1f;
            _tween = _canvasGroup.DOFade(0f, _durationOut).SetEase(_ease).SetDelay(_delay);
        }

        public void SetIn()
        {
            _canvasGroup.alpha = 1f;
        }
        
        public void SetOut()
        {
            _canvasGroup.alpha = 0f;
        }

    }
}