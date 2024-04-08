using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    [System.Serializable]
    public class CriticalDamageIndicator
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private float _duration;
        [SerializeField] private Vector2 _anchor1;
        [SerializeField] private Vector2 _anchor2;

        public void Animate()
        {
            _text.gameObject.SetActive(true);
            _text.DOKill();
            _text.rectTransform.DOKill();
            _text.rectTransform.localScale = Vector3.one;
            _text.alpha = 1f;
            _text.rectTransform.anchoredPosition = _anchor1;
            _text.rectTransform.DOAnchorPos(_anchor2, _duration);
            _text.rectTransform.DOScale(Vector3.one * 1.15f, _duration);
            _text.DOFade(0f, _duration).OnComplete(() =>
            {
                _text.gameObject.SetActive(false);
            });
        }
    }
    public class DamageHitsUI : MonoBehaviour, IDamageHitsUI
    {
        [SerializeField] private float _duration;
        [SerializeField] private List<TextMeshProUGUI> _texts;
        [SerializeField] private TextAppearance _mainAppearance;
        [SerializeField] private TextAppearance _critAppearance;
        [SerializeField] private CriticalDamageIndicator _criticalDamageIndicator;
        
        [System.Serializable]
        private class TextAppearance
        {
            public float scale;
            public Color color;            
            public float radius;
        }
        
        private int _index;
        private Camera _camera;
        
        private void OnEnable()
        {
            _camera =Camera.main;
        }


        public void ShowHit(Vector3 worldPos, float damage, bool isCrit)
        {
            var appearance = isCrit ? _critAppearance : _mainAppearance;
            var screenPos = _camera.WorldToScreenPoint(worldPos);
            if (_index >= _texts.Count)
                _index = 0;
            var t = _texts[_index];
            t.color = appearance.color;
            t.text = $"-{Mathf.RoundToInt(damage)}";
            t.DOKill();
            t.transform.localScale = Vector3.one * appearance.scale;
            t.transform.position = screenPos;
            t.gameObject.SetActive(true);
            var pos = screenPos + (Vector3)UnityEngine.Random.insideUnitCircle * appearance.radius;
            t.transform.DOMove(pos, _duration).OnComplete(() =>
            {
                t.gameObject.SetActive(false);
            });
            t.transform.DOScale(Vector3.one * (appearance.scale * .5f), _duration);
            _index++;
            if(isCrit)
                _criticalDamageIndicator.Animate();
        }
    }
}