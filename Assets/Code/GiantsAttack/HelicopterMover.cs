using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterMover : MonoBehaviour, IHelicopterMover
    {
        [SerializeField] private Transform _movable;      
        private float _t;
        private float _time;
        private CircularPath _path;
        private Coroutine _moving;
        
        public MoverSettings Settings { get; set; }
        public Transform LookAt { get; set; }
        
        public void SetPath(CircularPath path, Transform lookAtTarget)
        {
            _path = path;
            LookAt = lookAtTarget;
            _time = Mathf.Abs(path.endAngle - path.startAngle) / Settings.angularSpeed;
            CLog.LogRed($"Time set to {_time}");
        }

        public void BeginMovement()
        {
            StopMovement();
            _moving = StartCoroutine(MovingOnCircle());
        }

        public void StopMovement()
        {
            if(_moving != null)
                StopCoroutine(_moving);
        }

        private IEnumerator MovingOnCircle()
        {
            var elapsed = 0f;
            while (true)
            {
                while (_t <= 1f)
                {
                    SetPos(_t);
                    elapsed += Time.deltaTime;
                    _t = elapsed / _time;
                    yield return null;
                }
                _t = 1f;
                while (_t >= 0)
                {
                    SetPos(_t);
                    elapsed -= Time.deltaTime;
                    _t = elapsed / _time;
                    yield return null;
                }
                _t = 0;
            }
            
            void SetPos(float t)
            {
                var angle = Mathf.Lerp(_path.startAngle, _path.endAngle, t);
                var pos = _path.GetCirclePosAtAngle(angle);
                _movable.position = pos;
                var rot = Quaternion.LookRotation(LookAt.position - pos);
                _movable.rotation = rot;
            }
        }
    }
}