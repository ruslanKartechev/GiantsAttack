using DG.Tweening;
using UnityEngine;

namespace SleepDev.UIUtils
{
    public class SimplePopElement : PopElement
    {
        [SerializeField] private float _delay;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;
        [SerializeField] private float _scale = 1f;

        public override float Delay
        {
            get => _delay;
            set => _delay = value;
        }

        public override float Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public Ease Ease
        {
            get => _ease;
            set => _ease = value;
        }

        public float Scale
        {
            get => _scale;
            set => _scale = value;
        }
        
        public override void ScaleUp()
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one * _scale, _duration).SetEase(_ease).SetDelay(_duration);
        }
        
        public override void ScaleDown()
        {
            transform.localScale = Vector3.one * _scale;
            transform.DOScale(Vector3.zero, _duration).SetEase(_ease).SetDelay(_duration);
        }

    }
}