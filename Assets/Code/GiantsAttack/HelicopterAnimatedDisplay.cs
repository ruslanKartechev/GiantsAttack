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
        private Coroutine _working;

        private void Start()
        {
            Begin();
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