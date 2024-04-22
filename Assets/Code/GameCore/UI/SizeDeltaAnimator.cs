using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.UI
{
    public class SizeDeltaAnimator : MonoBehaviour
    {
        [SerializeField] private float _sizeDeltaChangeTime;
        [SerializeField] private Vector2 _startSizeDelta;
        [SerializeField] private Vector2 _endSizeDelta;
        [SerializeField] private List<RectTransform> _animated;
        private Coroutine _animating;
        
        public void Play()
        {
            _animating = StartCoroutine(Animating());
        }
        
        public IEnumerator Animating()
        {
            var elapsed = 0f;
            var time = _sizeDeltaChangeTime;
            while (elapsed < time)
            {
                var delta = Vector2.Lerp(_startSizeDelta, _endSizeDelta, elapsed / time);
                foreach (var im in _animated)
                    im.sizeDelta = delta;
                elapsed += Time.deltaTime;
                yield return null;
            }
            foreach (var im in _animated)
                im.sizeDelta = _endSizeDelta;
        }
    }
}