using System;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using TMPro;
using UnityEngine;

namespace GameCore.UI
{
    public class DamageHitsUI : MonoBehaviour, IDamageHitsUI
    {
        [SerializeField] private float _duration;
        [SerializeField] private List<TextMeshProUGUI> _texts;
        [SerializeField] private TextAppearance _mainAppearance;
        [SerializeField] private TextAppearance _critAppearance;
        [SerializeField] private TextAppearance _headShotAppearance;
        [SerializeField] private CriticalDamageIndicator _criticalDamageIndicator;
        [SerializeField] private CriticalDamageIndicator _headShotDamageIndicator;
        [SerializeField] private List<RectTransform> _crisUiPoints;

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
            _camera = Camera.main;
        }

        private void Awake()
        {
            _criticalDamageIndicator.Hide();
            _headShotDamageIndicator.Hide();
        }

        public void ShowHit(Vector3 worldPos, float damage, DamageIndicationType type)
        {
            var appearance = _mainAppearance;
            switch (type)
            {
                case DamageIndicationType.Critical:
                    appearance = _critAppearance;
                    _criticalDamageIndicator.SetPoint(_crisUiPoints.Random());
                    _criticalDamageIndicator.Animate();
                    break;
                case DamageIndicationType.Headshot:
                    appearance = _headShotAppearance;
                    _headShotDamageIndicator.Animate();
                    break;
            }
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
        }
    }
}