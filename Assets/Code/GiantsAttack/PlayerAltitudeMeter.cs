using System.Collections;
using UnityEngine;

namespace GiantsAttack
{
    public class PlayerAltitudeMeter : MonoBehaviour
    {
        [SerializeField] private Transform _arrow;
        [SerializeField] private float _time;
        [SerializeField] private Vector2 _minMaxAngles;
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
                yield return AngleChange(_minMaxAngles.x, _minMaxAngles.y);
                yield return AngleChange(_minMaxAngles.y, _minMaxAngles.x);
                yield return null;
            }
        }

        private IEnumerator AngleChange(float from , float to)
        {
            var elapsed = Time.deltaTime;
            var time = _time;
            var t = elapsed / time;
            while (t <= 1f)
            {
                var a = Mathf.Lerp(from, to, t);
                var angles = _arrow.localEulerAngles;
                angles.z = a;
                _arrow.localEulerAngles = angles;
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }

        }
    }
}