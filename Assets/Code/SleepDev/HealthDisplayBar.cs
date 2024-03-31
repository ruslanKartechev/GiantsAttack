using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class HealthDisplayBar : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UnityEngine.UI.Image _fillImage;
        [SerializeField] private UnityEngine.UI.Image _fillImageBack;
        [SerializeField] private float _defaultTime;
        
        private Coroutine _filling;
        
        public void Show()
        {
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _canvas.enabled = false;
            StopUpdate();
        }

        public void SetHealth(float val)
        {
            StopUpdate();
            _fillImageBack.fillAmount = _fillImage.fillAmount = val;
        }

        public void UpdateHealth(float val)
        {
            StopUpdate();
            _fillImage.fillAmount = val;
            _filling = StartCoroutine(Filling(val, _defaultTime));
        }

        private void StopUpdate()
        {
            if(_filling != null)
                StopCoroutine(_filling);
        }
        
        private IEnumerator Filling(float endVal, float time)
        {
            var elapsed = Time.unscaledDeltaTime;
            var t = elapsed / time;
            var startval = _fillImageBack.fillAmount;
            while (t <= 1f)
            {
                _fillImageBack.fillAmount = (Mathf.Lerp(startval, endVal, t));
                elapsed += Time.unscaledDeltaTime;
                t = elapsed / time;
                yield return null;
            }
            _fillImageBack.fillAmount = endVal;

        }
        
    }
}