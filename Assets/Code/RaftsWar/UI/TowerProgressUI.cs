using System;
using System.Collections;
using DG.Tweening;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaftsWar.UI
{
    public class TowerProgressUI : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private Image _background;
        [SerializeField] private float _animateTime = .33f;
        [SerializeField] private TextMeshProUGUI _text;
        [Space(20)]
        [SerializeField] private float _punchTime;
        [SerializeField] private float _punchScale;
        [SerializeField] private Ease _punchEase;
        [SerializeField] private GameObject _shining;        

        private Coroutine _animating;
        private Action _callback;
        
        public void SetCurrentIconAndFill(SpriteFillIcon icon, float progress, bool showText = true)
        {
            _shining.gameObject.SetActive(false);
            SetCurrentIcon(icon);
            _fill.fillAmount = progress;
            SetFill(progress);
            _text.enabled = showText;
        }
        
        
        public void AnimateProgress(float val1, float val2, SpriteFillIcon icon, Action callback)
        {
            SetCurrentIcon(icon);
            SetFill(val1);
            _callback = callback;
            if(_animating != null)
                StopCoroutine(_animating);
            _animating = StartCoroutine(Animating(val1, val2, Callback));
        }

        public void AnimateUpgrade()
        {
            _shining.gameObject.SetActive(true);
            transform.DOPunchScale(_punchScale * Vector3.one, _punchTime).SetEase(_punchEase);
        }
        
        private void Callback()
        {
            _callback?.Invoke();
        }

        private void SetCurrentIcon(SpriteFillIcon icon)
        {
            _background.sprite = icon.background;
            _fill.sprite = icon.mainIcon;
        }


        private IEnumerator Animating(float val1, float val2, Action onDone)
        {
            var elapsed = Time.deltaTime;
            var time = _animateTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                var val = Mathf.Lerp(val1, val2, t);
                SetFill(val);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            SetFill(val2);
            onDone.Invoke();
        }
        
        private void SetFill(float amount)
        {
            _text.text = $"{(int)(amount * 100)}%";
            _fill.fillAmount = amount;
        }
    }
}