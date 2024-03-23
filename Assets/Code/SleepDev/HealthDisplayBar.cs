using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class HealthDisplayBar : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UnityEngine.UI.Image _fillImage;
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
            SetVal(val);
        }

        public void UpdateHealth(float val)
        {
            StopUpdate();
            _filling = StartCoroutine(Filling(val, _defaultTime));
        }

        private void StopUpdate()
        {
            if(_filling != null)
                StopCoroutine(_filling);
        }
        
        private IEnumerator Filling(float endVal, float time)
        {
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var startval = _fillImage.fillAmount;
            while (t <= 1f)
            {
                SetVal(Mathf.Lerp(startval, endVal, t));
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            SetVal(endVal);
        }

        private void SetVal(float val)
        {
            _fillImage.fillAmount = val;

        }
    }
}