using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GameCore.UI
{
    public class ObjectiveUI : MonoBehaviour
    {
        [SerializeField] private TextByCharPrinter _printer1;
        [SerializeField] private TextByCharPrinter _printer2;
        [Space(10)] 
        [SerializeField] private float _printer2Delay;
        [Space(10)] 
        [SerializeField] private SizeDeltaAnimator _deltaAnimator;
        [Space(10)] 
        [SerializeField] private float _hideDelay;
        [SerializeField] private float _hideDuration;
        [SerializeField] private Transform _hideScalable;
        private Coroutine _animating;

        public void SetObjectiveText(string txt)
        {
            _printer2.Text = txt;
        }
        
        public void Play(Action callback)
        {
            gameObject.SetActive(true);
            _animating = StartCoroutine(Animating(callback));
        }
        
        private IEnumerator Animating(Action callback)
        {
            yield return _deltaAnimator.Animating();
            _printer1.PrintText();
            yield return new WaitForSeconds(_printer2Delay);
            _printer2.PrintText();
            yield return new WaitForSeconds(_hideDelay);
            _hideScalable.DOScale(new Vector3(1f, 0f, 1f), _hideDuration).OnComplete(() =>
            {
                callback?.Invoke();
                gameObject.SetActive(false);
            });
        }
        

    }
}