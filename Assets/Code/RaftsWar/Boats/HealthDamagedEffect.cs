using System;
using DG.Tweening;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class HealthDamagedEffect : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _damagedText;
        [SerializeField] private Vector2 _anchoredStart;
        [SerializeField] private Vector2 _anchoredEnd;
        [SerializeField] private float _duration;

        private void Start()
        {
            _damagedText.enabled = false;
        }

        public void Play(float damageAmount)
        {
            _damagedText.gameObject.SetActive(true);
            _damagedText.enabled = true;
            _damagedText.text = $"-{damageAmount}";
            _damagedText.DOKill();
            _damagedText.rectTransform.DOKill();
            _damagedText.alpha = 1f;
            _damagedText.rectTransform.anchoredPosition = _anchoredStart;
            _damagedText.rectTransform.DOAnchorPos(_anchoredEnd, _duration);
            _damagedText.DOFade(0f, _duration);
        }

    }
}