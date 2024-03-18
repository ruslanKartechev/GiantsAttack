using System.Collections;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerRadius : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Transform _scalable;
        private const string ThresholdKey = "_Threshold";
        private const float ThresholdMin = 0.48f;
        private const float ThresholdMax = 0.495f;
        private const float RadMin = 20f;
        private const float RadMax = 60f;

        private Coroutine _changing;

        public void SetView(Material material)
        {
            _renderer.sharedMaterial = material;
        }
        
        public void SetRadius(float radius)
        {
            // CLog.LogBlue($"[RADIUS] SET TO RADIUS");
            _scalable.localScale = Vector3.one * radius;
            UpdateThreshold(radius);
        }
        
        public void UpdateRadius(float radius)
        {
            // CLog.LogBlue($"[RADIUS] updating to radius");
            UpdateThreshold(radius);
            Stop();
            _changing = StartCoroutine(Changing(_scalable.localScale.x, radius));
        }

        private void UpdateThreshold(float radius)
        {
            var t = Mathf.InverseLerp(RadMin, RadMax, radius);
            var val = Mathf.Lerp(ThresholdMin, ThresholdMax, t);
            _renderer.sharedMaterial.SetFloat(ThresholdKey, val);
        }

        public void Show()
        {
            // CLog.LogBlue($"[RADIUS] Show");
            _renderer.enabled = true;
        }

        public void Hide()
        {
            // CLog.LogBlue($"[RADIUS] HIDE");
            _renderer.enabled = false;
        }

        public void ScaleToHide()
        {
            // CLog.LogBlue($"[RADIUS] Scale to hide");
            if (_renderer.enabled == false)
                return;
            StopAllCoroutines();
            StartCoroutine(ScalingToHide());
        }

        private void Stop()
        {
            if(_changing != null)
                StopCoroutine(_changing);
        }

        private IEnumerator Changing(float from, float to)
        {
            yield return null;
            var time = GlobalConfig.RadiusChangeTime;
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
                var s=  Mathf.Lerp(from, to, pt);
                _scalable.localScale = Vector3.one * s;
            }
        }

        private IEnumerator ScalingToHide()
        {
            var from = _scalable.localScale.x;
            var to = .1f;
            var time = .2f;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t < 1f)
            {
                Set(t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            _renderer.enabled = false;
            void Set(float pt)
            {
                var s=  Mathf.Lerp(from, to, pt);
                _scalable.localScale = Vector3.one * s;
            }
        }
    }
}