using System.Collections;
using System.Collections.Generic;
using SleepDev;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RaftsWar.Boats
{
    public class BoatPartsProgBar : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TextMeshProUGUI _text;
        [Space(10)]
        [SerializeField] private Image _fill;
        [SerializeField] private List<Image> _imagesToColor;
        private Coroutine _changing;

        public void SetCount(int num, int outOf)
        {
            _text.text = $"{num}/{outOf}";
        }

        public void SetFill(float amount)
        {
            _fill.fillAmount = amount;
        }

        public void UpdateFill(float amount)
        {
            Stop();
            _changing = StartCoroutine(Changing(_fill.fillAmount, amount));
        }

        public void SetLevel(int level)
        {
            _text.text = (level + 1).ToString();
        }

        public void Reset()
        {
            Stop();
            _fill.fillAmount = 0f;
        }

        public void On()
        {
            _canvas.enabled = true;
        }

        public void Off()
        {
            _canvas.enabled = false;
        }

        public void SetView(TowerSettings towerSettings)
        {
            foreach (var image in _imagesToColor)
                image.color =  towerSettings.uiColor;
        }

        private void Stop()
        {
            if (_changing != null)
                StopCoroutine(_changing);
        }

        private IEnumerator Changing(float from, float to)
        {
            yield return null;
            var time = GlobalConfig.TowerProgressFillTime;
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
                _fill.fillAmount = Mathf.Lerp(from, to, pt);
            }
        }
    }
}

    