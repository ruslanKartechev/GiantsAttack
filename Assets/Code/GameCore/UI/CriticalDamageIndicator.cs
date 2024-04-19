using DG.Tweening;
using UnityEngine;

namespace GameCore.UI
{
    [System.Serializable]
    public class CriticalDamageIndicator
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Vector3 _punchScale;
        [SerializeField] private float _scaleTime;
        [SerializeField] private float _fadeTime;
        [SerializeField] private float _defaultScale = 1f;
        [SerializeField] private float _angleRange = 10;
        [SerializeField] private CanvasGroup _group;

        public void SetPoint(RectTransform point)
        {
            _rect.anchoredPosition = point.anchoredPosition;
        }

        public void Hide()
        {
            _rect.gameObject.SetActive(false);
        }
        
        public void Animate()
        {
            _rect.DOKill();
            _group.DOKill();
            
            _rect.gameObject.SetActive(true);
            var angles = _rect.localEulerAngles;
            angles.z = UnityEngine.Random.Range(-_angleRange, _angleRange);
            _rect.localEulerAngles = angles;
            
            _group.alpha = 1f;
            _rect.localScale = new Vector3(_defaultScale,_defaultScale,_defaultScale);
            _rect.DOPunchScale(_punchScale, _scaleTime);
            _group.DOFade(0f, _fadeTime).OnComplete(() =>
            {
                _rect.gameObject.SetActive(false);
            });
        }
    }
}