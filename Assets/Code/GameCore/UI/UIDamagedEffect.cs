using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    public class UIDamagedEffect : MonoBehaviour, IUIDamagedEffect
    {
        [SerializeField] private float _fullAlpha = 1f;
        [SerializeField] private UIDamageEffectSettings _shortSettings;
        [SerializeField] private UIDamageEffectSettings _longSettings;
        [SerializeField] private Image _image;
        private Coroutine _working;
        private bool _isPlayingShort;
        private bool _isPlayingLong;
        
        public void PlayLong()
        {
            if (_isPlayingLong)
                return;
            Stop();
            _isPlayingLong = true;
            _working = StartCoroutine(Working(_longSettings));
        }
        
        public void PlayShort()
        {
            if (_isPlayingShort)
                return;
            Stop();
            _isPlayingShort = true;
            _working = StartCoroutine(Working(_shortSettings));
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
            _isPlayingShort = false;
            _isPlayingLong = false;
        }

        private IEnumerator Working(UIDamageEffectSettings settings)
        {
            _image.enabled = true;
            for (var i = 0; i < settings.count; i++)
            {
                yield return Fading(0f, _fullAlpha, settings.fadeDuration);
                yield return Fading(_fullAlpha, 0f, settings.fadeDuration);
            }
            _image.enabled = false;
            _isPlayingShort = false;
            _isPlayingLong = false;
        }

        private IEnumerator Fading(float f1, float f2, float time)
        {
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                var col = _image.color;
                col.a = Mathf.Lerp(f1, f2, t);
                _image.color = col;
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
        }
    }
}