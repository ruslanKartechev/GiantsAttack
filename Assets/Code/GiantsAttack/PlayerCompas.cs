using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class PlayerCompas : MonoBehaviour
    {
        [SerializeField] private Transform _arrow;
        [SerializeField] private Transform _body;
        private Coroutine _working;
        
        public void BeginTracking(Transform point)
        {
            _working = StartCoroutine(Working(point));
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        private IEnumerator Working(Transform point)
        {
            while (true)
            {
                var dir = (point.position - _body.position).XZPlane();
                var angle = Vector3.Angle(_body.forward, dir);
                var eulers = _arrow.localEulerAngles;
                eulers.y = angle;
                _arrow.localEulerAngles = eulers;
                yield return null;
            }
        }
    }
}