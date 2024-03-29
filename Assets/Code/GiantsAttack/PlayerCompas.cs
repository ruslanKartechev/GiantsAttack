using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class PlayerCompas : MonoBehaviour
    {
        [SerializeField] private Transform _arrow;
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
                var dir = (point.position - transform.position).XZPlane();
                var angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
                var eulers = _arrow.localEulerAngles;
                eulers.y = angle;
                _arrow.localEulerAngles = eulers;
                // _arrow.localRotation = Quaternion.AngleAxis(angle, _arrow.up);
                yield return null;
            }
        }
    }
}