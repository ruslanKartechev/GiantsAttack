using System.Collections;
using SleepDev;
using UnityEngine;
using System;

namespace RaftsWar.Cam
{
    public class PlayerCamera : MonoBehaviour, IPlayerCamera
    {
        [SerializeField] private CameraShaker _shaker;
        [SerializeField] private Transform _movable;
        private CameraCommandsHolder _commandsHolder;
        private Vector3 _followOffset;
        private Coroutine _processing;
        private bool _isExecuting;

        private void Awake()
        {
            _commandsHolder = new CameraCommandsHolder();
        }

        private void CallNextCommand()
        {
            if (_commandsHolder.Count > 0)
            {
                _isExecuting = true;
                _commandsHolder.GetLast().Execute(this, CallNextCommand);
            }
            else
                _isExecuting = false;
        }

        public void AddCommand(ICommand<IPlayerCamera> command)
        {
            // CLog.LogYellow($"Added a command {command.GetType()}");
            _commandsHolder.Add(command);
            if (_isExecuting == false)
                CallNextCommand();
        }

        public void SetPoint(Transform point)
        {
            SetPosition(point);
            SetRotation(point);
        }        
        
        public void SetPosition(Transform point)
        {
            _movable.position = point.position;
        }
        
        public void SetRotation(Transform point)
        {
            _movable.rotation = point.rotation;
        }
        
        public void MoveToPoint(Transform point, float time, Action onEnd)
        {
            _movable.parent = null;
            _processing = StartCoroutine(MovingToPoint(point, time, onEnd));
        }

        public void MoveToPointLocal(Transform parent, Transform point, float time, Action callback)
        {
            StopMoving();
            _processing = StartCoroutine(TransitioningToLocal(point,  
                parent, time, callback));
        }

        public void Wait(float time, Action onEnd)
        {
            StopMoving();
            _processing = StartCoroutine(Waiting(time, onEnd));
        }

        public void StopMoving()
        {
            if(_processing != null)
                StopCoroutine(_processing);
        }


        public void Shake()
        {
            _shaker.PlayDefault();
        }
        
        private IEnumerator MovingToPoint(Transform followPoint, float time, Action onEnd)
        {
            var pos1 = _movable.position;
            var rot1 = _movable.rotation;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                _movable.position = Vector3.Lerp(pos1, followPoint.position, t);
                _movable.rotation = Quaternion.Lerp(rot1, followPoint.rotation, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            _movable.position = followPoint.position;
            _movable.rotation = followPoint.rotation;
            onEnd?.Invoke();
        }
        
        private IEnumerator TransitioningToLocal(Transform followPoint, 
            Transform parent, float time, Action onEnd)
        {
            _movable.parent = parent;
            var pos1 = _movable.localPosition;
            var rot1 = _movable.localRotation;
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            while (t <= 1f)
            {
                _movable.localPosition = Vector3.Lerp(pos1, followPoint.localPosition, t);
                _movable.localRotation = Quaternion.Lerp(rot1, followPoint.localRotation, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            _movable.localPosition = followPoint.localPosition;
            _movable.localRotation = followPoint.localRotation;
            onEnd?.Invoke();
        }

        private IEnumerator Waiting(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
        
        private IEnumerator Following(Transform followPoint)
        {
            while (true)
            {
                _movable.CopyPosRot(followPoint);
                yield return null;
            } 
        }
        
    }
}