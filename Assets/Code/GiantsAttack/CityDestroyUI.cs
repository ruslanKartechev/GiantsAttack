using System;
using System.Collections;
using System.Collections.Generic;
using GameCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GiantsAttack
{
    public class CityDestroyUI : MonoBehaviour, ITargetsCountUI
    {
        [SerializeField] private SizeDeltaAnimator _sizeDeltaAnimator;
        [Space(10)] 
        [SerializeField] private float _fillTime;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private List<Image> _fillImages;
        private int _maxCount;
        private float _currentPercent;
        private Coroutine _filling;

        public void Show(Action onEnd)
        {
            gameObject.SetActive(true);
            _sizeDeltaAnimator.Play();
        }

        public void SetCount(int max, int current)
        {
            _maxCount = max;
            SetPercentToCount(current);
        }

        public void UpdateCount(int count)
        {
            if(_filling != null)
                StopCoroutine(_filling);
            _filling = StartCoroutine(Filling((float)count/_maxCount));
        }

        public void SetActive(bool show)
        {
            gameObject.SetActive(show);
        }
        
        private void SetPercentToCount(int count)
        {
            _currentPercent = (float)count / _maxCount;
            SetPercent(_currentPercent);
        }

        private void SetPercent(float percent)
        {
            var multPercent = Mathf.RoundToInt(percent * 100);
            _countText.text = $"{multPercent}%";
            foreach (var im in _fillImages)
                im.fillAmount = percent;
        }

        private IEnumerator Filling(float targetPercent)
        {
            var elapsed = 0f;
            var start = _currentPercent;
            while (elapsed < _fillTime)
            {
                _currentPercent = Mathf.Lerp(start, targetPercent, elapsed / _fillTime);
                SetPercent(_currentPercent);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _currentPercent = targetPercent;
            SetPercent(_currentPercent);
        }
        
    }
}