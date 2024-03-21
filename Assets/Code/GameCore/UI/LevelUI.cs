using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCore.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private float _animDelay = .25f;
        [SerializeField] private float _failFill = 0.239f;
        [SerializeField] private float _scale = 0.239f;
        [SerializeField] private float _scaleTime = 0.239f;
        [SerializeField] private float _scaleDelay = .1f;
        [SerializeField] private float _fillTime = 1f;
        [SerializeField] private TextMeshProUGUI _prevLevel;
        [SerializeField] private TextMeshProUGUI _nextLevel;
        [SerializeField] private List<RectTransform> _points;
        [SerializeField] private List<RectTransform> _greenPoints;
        [SerializeField] private RectTransform _cross;
        [SerializeField] private Image _fill;

        public void SetLevelFromPlayerData()
        {
#if UNITY_EDITOR
            if (GCon.PlayerData == null)
            {
                SetLevel(0);
                return;
            }
#endif
            var level = GCon.PlayerData.LevelTotal;
            SetLevel(level);
        }
        
        public void SetLevel(int level)
        {
            _prevLevel.text = $"{level+1}";
            _nextLevel.text = $"{level+2}";
        }

        private IEnumerator AnimatingWin()
        {
            yield return new WaitForSeconds(_animDelay);
            _fill.fillAmount = 0f;
            _fill.DOFillAmount(1f, _fillTime);
            _prevLevel.transform.DOPunchScale(Vector3.one * _scale, _scaleTime);
            for (var i = 0; i < _greenPoints.Count; i++)
            {
                var p = _greenPoints[i];
                p.gameObject.SetActive(true);
                p.DOPunchScale(Vector3.one * _scale, _scaleTime);
                yield return new WaitForSeconds(_scaleDelay);
            }
            _nextLevel.transform.DOPunchScale(Vector3.one * _scale, _scaleTime);
        }
        
        public void AnimateWin()
        {
            _cross.gameObject.SetActive(false);
            StartCoroutine(AnimatingWin());
        }

        public void AnimateFail()
        {
            _cross.gameObject.SetActive(true);
            _cross.DOPunchScale(Vector3.one * _scale, _scaleTime);
            _fill.fillAmount = 0f;
            _fill.DOFillAmount(_failFill, _fillTime/2f);
        }
    }
}