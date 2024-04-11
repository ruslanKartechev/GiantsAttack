using System;
using System.Collections;
using GameCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace GiantsAttack
{
    public class CityDestroyUI : MonoBehaviour, ITargetsCountUI
    {
        [SerializeField] private Animator _titleAnimator;
        [SerializeField] private Animator _countAnimator;
        [SerializeField] private float _countAnimatorDelay;
        [SerializeField] private float _endDelay;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Animator _highlightAnimator;
        private int _maxCount;
        
        public void Show(Action onEnd)
        {
            gameObject.SetActive(true);
            StartCoroutine(Working(onEnd));
        }

        public void SetCount(int max, int current)
        {
            _maxCount = max;
            _countText.text = $"{current}/{max}";
        }

        public void UpdateCount(int count)
        {
            _countText.text = $"{count}/{_maxCount}";
        }

        public void SetActive(bool show)
        {
            gameObject.SetActive(show);
        }

        public void Highlight()
        {
            _highlightAnimator.gameObject.SetActive(true);
            _highlightAnimator.enabled = true;
        }

        private IEnumerator Working(Action onEnd)
        {
            _titleAnimator.gameObject.SetActive(true);
            _titleAnimator.enabled = true;
            yield return new WaitForSeconds(_countAnimatorDelay);
            _countAnimator.gameObject.SetActive(true);
            _countAnimator.enabled = true;
            yield return new WaitForSeconds(_endDelay);
            _titleAnimator.gameObject.SetActive(false);
            onEnd?.Invoke();
        }
    }
}