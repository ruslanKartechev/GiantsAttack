using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CameraRailMover : MonoBehaviour
    {
        [SerializeField] private Transform _movable;
        [SerializeField] private Transform _lookAt;
        [SerializeField] private Transform _p1;
        [SerializeField] private Transform _p2;
        [SerializeField] private Transform _p3;
        [SerializeField] private float _time;
        [SerializeField] private bool _moveBackwardsToo;
        private Coroutine _working;

        #if UNITY_EDITOR
        public bool drawGizmos = true;
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;
            var count = 30f;
            for (var i = 1; i <= count; i++)
            {
                var t1 = i / count;
                var t2 = (i - 1) / count;
                var p1 =  Bezier.GetPosition(_p1.position, _p2.position, _p3.position, t1);
                var p2 =  Bezier.GetPosition(_p1.position, _p2.position, _p3.position, t2);
                Debug.DrawLine(p1,p2, Color.white, .25f);
            }
        }

        [ContextMenu("Look at")]
        public void LookAt()
        {
            _p1.rotation = Quaternion.LookRotation(_lookAt.position - _p1.position);
            _p2.rotation = Quaternion.LookRotation(_lookAt.position - _p2.position);
            _p3.rotation = Quaternion.LookRotation(_lookAt.position - _p3.position);
        }
        
        #endif

        public void Move(Action onEnd = null)
        {
            Stop();
            _working = StartCoroutine(Moving(onEnd));
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private IEnumerator Moving(Action onEnd = null)
        {
            yield return MovingBezier(_p1, _p2, _p3, _time);
            if (_moveBackwardsToo)
            {
                yield return MovingBezier(_p3, _p2, _p1, _time);
            }
            onEnd?.Invoke();
        }

        private IEnumerator MovingBezier(Transform p1, Transform p2, Transform p3, float time)
        {
            var elapsed = 0f;
            var t = elapsed / time;
            while (t <= 1f)
            {
                var p = Bezier.GetPosition(p1.position, p2.position, p3.position, t);
                var rot = Quaternion.LookRotation((_lookAt.position - p));
                _movable.SetPositionAndRotation(p, rot);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            {
                t = 1f;
                var p = Bezier.GetPosition(p1.position, p2.position, p3.position, t);
                var rot = Quaternion.LookRotation((_lookAt.position - p));
                _movable.SetPositionAndRotation(p, rot);
            }
        }
    }
}