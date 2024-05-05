using System;
using System.Collections;
using DG.Tweening;
using SleepDev;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class TutorialUI : MonoBehaviour
    {
        [SerializeField] private Image _squareAim;
        [SerializeField] private Vector2 _size1;
        [SerializeField] private Vector2 _size2;
        [SerializeField] private float _sizeAnimTime;
        [Space(10)] 
        [SerializeField] private ProperButton _inputBtn;
        [SerializeField] private TutorHandMover _handMover;
        [SerializeField] private TextByCharPrinter _byCharPrinter;
        [SerializeField] private GameObject _prompt;
        [SerializeField] private GameObject _handArea;
        [SerializeField] private float _handAreaShowTime;
        private Coroutine _animating;
        private Coroutine _animating2;
            
        public TutorHandMover Hand => _handMover;
        public TextByCharPrinter TextPrinter => _byCharPrinter;
        public ProperButton InputBtn => _inputBtn;

        public void ShowHandAreaAnimated()
        {
            _prompt.SetActive(true);
            _handArea.SetActive(true);
            _handMover.ShowHandAnimation();
            if(_animating2 != null)
                StopCoroutine(_animating2);
            _animating2 = StartCoroutine(HandAreaAnimating(0f, 1f));
        }

        public void HideHandAreaAnimated()
        {
            if(_animating2 != null)
                StopCoroutine(_animating2);
            _animating2 = StartCoroutine(HandAreaAnimating(1f, 0f, () =>
            {
                _handMover.HideHand();
                _handArea.SetActive(false);
            }));
        }

        private IEnumerator HandAreaAnimating(float scale1, float scale2, Action onEnd = null)
        {
            var elapsed = 0f;
            var x = 0f;
            while (elapsed < _handAreaShowTime)
            {
                x = Mathf.Lerp(scale1, scale2, elapsed / _handAreaShowTime);
                var scale = new Vector3(x, 1f, 1f);
                _prompt.transform.localScale = _handArea.transform.localScale = scale;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            _prompt.transform.localScale = _handArea.transform.localScale = new Vector3(scale2, 1f,1f);
            onEnd?.Invoke();
        }
        
        public void ShowSquareAim()
        {
            _squareAim.gameObject.SetActive(true);
            _animating = StartCoroutine(AimAnimating());
        }

        public void HideAim()
        {
            _squareAim.enabled = false;
            if (_animating != null)
                StopCoroutine(_animating);
        }

        
        private IEnumerator AimAnimating()
        {
            while (true)
            {
                yield return SizeDeltaChanging(_size1, _size2);
                yield return SizeDeltaChanging(_size2, _size1);
            }
        }

        private IEnumerator SizeDeltaChanging(Vector2 from, Vector2 to)
        {
            var elapsed = 0f;
            var time = _sizeAnimTime;
            while (elapsed < _sizeAnimTime)
            {
                _squareAim.rectTransform.sizeDelta = Vector2.Lerp(from, to, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _squareAim.rectTransform.sizeDelta = to;
        }

    }
}