using DG.Tweening;
using UnityEngine;

namespace SleepDev.UIUtils
{
    public class SimplePopElement : PopElement
    {
        [SerializeField] private float _delay;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;

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

        public override void ScaleUp()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, _duration).SetEase(_ease).SetDelay(_duration);
        }
        
    }
}