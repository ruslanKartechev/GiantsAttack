using DG.Tweening;
using UnityEngine;

namespace SleepDev
{
    public class JoystickUI : MonoBehaviour, IJoystickUI
    {
        [SerializeField] private float _maxRad;
        [SerializeField] private float _fadeTime;
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _block;
        [SerializeField] private CanvasGroup _group;
        
        public void Show()
        {
            _group.gameObject.SetActive(true);
            _group.alpha = 0f;
            _group.DOKill();
            _group.DOFade(1f, _fadeTime);
        }

        public void Hide()
        {
            _group.DOKill();
            _group.DOFade(0f, _fadeTime).OnComplete(() => { _group.gameObject.SetActive(false); });
        }

        public void SetPosition(Vector3 screenPosition)
        {
            _block.position = screenPosition;
        }

        public void SetJoystickLocal(Vector3 delta)
        {
            _movable.localPosition = delta;
        }

        public void SetJoystickLocal(Vector3 normalizedPos, float percent)
        {
            _movable.localPosition = normalizedPos * percent * _maxRad;
        }
    }
}