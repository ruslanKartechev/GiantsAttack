using System.Collections;
using UnityEngine;

namespace GiantsAttack
{
    public class PlayerAltitudeMeter : MonoBehaviour
    {
        [SerializeField] private Transform _arrow;
        [SerializeField] private AltitudeMeterSettings _settings;

        private Coroutine _working;
        
        public void Begin()
        {
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
                var t = Mathf.InverseLerp(_settings.altitudeLimits.x, _settings.altitudeLimits.y, transform.position.y);
                var a = Mathf.Lerp(_settings.angleLimits.x, _settings.angleLimits.y, t);
                var angles = _arrow.localEulerAngles;
                angles.z = a;
                _arrow.localEulerAngles = angles;
                yield return null;
            }
        }
    }
}