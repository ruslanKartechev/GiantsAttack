using System.Collections;
using TMPro;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterAnimatedDisplay : MonoBehaviour
    {
        [SerializeField] private float _duration;
        [SerializeField] private Vector2 _alphaLimits;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextMeshProUGUI[] _bulletsCountTexts;
        [SerializeField] private Color _colorLow;
        [SerializeField] private Color _colorNormal;
        
        
        private Coroutine _working;

        private void Start()
        {
            Begin();
        }

        public void ShowLowCount(byte index)
        {
            _bulletsCountTexts[index].color = _colorLow;
        }
        
        public void ShowNormalCount(byte index)
        {
            _bulletsCountTexts[index].color = _colorNormal;
        }


        public void Begin()
        {
            Stop();
            _working = StartCoroutine(Working());
        }
        
        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        public void SetBulletsCount(byte index, byte count)
        {
            _bulletsCountTexts[index].text = $"{count}";
        }
        
        public void SetCountLeftRight(byte leftCount, byte rightCount)
        {
            _bulletsCountTexts[0].text = $"{leftCount}";
            _bulletsCountTexts[1].text = $"{rightCount}";
        }
        
        public void SetCountLeft(byte count)
        {
            _bulletsCountTexts[0].text = $"{count}";
        }

        public void SetCountRight(byte count)
        {
            _bulletsCountTexts[1].text = $"{count}";
        }

        private IEnumerator Working()
        {
            while (true)
            {
                var elapsed = 0f;
                while (elapsed < _duration)
                {
                    _text.alpha = Mathf.Lerp(_alphaLimits.x, _alphaLimits.y, elapsed / _duration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                elapsed = 0f;
                while (elapsed < _duration)
                {
                    _text.alpha = Mathf.Lerp(_alphaLimits.y, _alphaLimits.x, elapsed / _duration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                yield return null;
            }
        }
        
        
    }
}