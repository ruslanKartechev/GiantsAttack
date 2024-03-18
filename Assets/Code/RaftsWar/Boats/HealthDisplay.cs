using System.Collections;
using SleepDev;
using TMPro;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private HealthDamagedEffect _damagedEffect;
        
        private Coroutine _changing;
        private float _current;

        public void SetFill(float amount)
        {
            _current = amount;
            _text.text = $"{(int)(_current * 100f)}";
        }

        public void UpdateFill(float amount)
        {
            Stop();
            _changing = StartCoroutine(Changing(_current, amount));
        }

        public void Reset()
        {
            _current = 0f;
            SetFill(_current);
        }

        public void On()
        {
            gameObject.SetActive(true);
            _canvas.enabled = true;
        }
        
        public void Off()
        {
            _canvas.enabled = false;
        }

        public void PlayDamaged(float damageAmount) => _damagedEffect.Play(damageAmount);

        private void Stop()
        {
            if(_changing != null)
                StopCoroutine(_changing);
        }

        private IEnumerator Changing(float from, float to)
        {
            yield return null;
            var time = GlobalConfig.HealthFillTime;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t < 1f)
            {
                Set(t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            Set(1f);
            void Set(float pt)
            {
                // _fill.fillAmount = Mathf.Lerp(from, to, pt);
                var a = Mathf.Lerp(from, to, pt);
                _text.text = $"{(int)(a * 100f)}";
                _current = a;
            }
            // CLog.Log($"disabled ...");
            // _damagedText.enabled = false;
        }
    }
}