using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace GameCore.Cam
{
    [DefaultExecutionOrder(1000)]
    public class PlayerCamera : MonoBehaviour, IPlayerCamera
    {
        [SerializeField] private CameraShaker _shaker;
        [SerializeField] private Transform _movable;
        private CameraCommandsHolder _commandsHolder;
        private Vector3 _followOffset;
        private Coroutine _processing;
        private Coroutine _following;
        private bool _isExecuting;

        private void Awake()
        {
            _commandsHolder = new CameraCommandsHolder();
            CameraContainer.PlayerCamera = this;
            CameraContainer.MainCam = Camera.main;
            CameraContainer.Shaker = _shaker;
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
            StopMoving();
            _processing = StartCoroutine(MovingToPoint(point, time, onEnd));
        }

        public void MoveToPointToFollow(Transform point, float time, Action callback)
        {
            StopMoving();
            _processing = StartCoroutine(TransitioningToFollow(point, time, callback));
        }

        public void MoveToPointToParent(Transform point, float time, Action callback)
        {
            StopMoving();
            _processing = StartCoroutine(TransitioningToParent(point, time, callback));
        }

        public void Wait(float time, Action onEnd)
        {
            StopMoving();
            _processing = StartCoroutine(Waiting(time, onEnd));
        }

        public void StopMoving()
        {
            StopFollowing();
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

        private void StopFollowing()
        {
            if(_following != null)
                StopCoroutine(_following);
        }
        
        private IEnumerator TransitioningToFollow(Transform followPoint, float time, Action onEnd)
        {
            yield return MovingToPoint(followPoint, time, onEnd);
            StopFollowing();
            _following = StartCoroutine(Following(followPoint));
        }
        
        private IEnumerator TransitioningToParent(Transform followPoint, float time, Action onEnd)
        {
            yield return MovingToPoint(followPoint, time, onEnd);
            StopFollowing();
            transform.parent = followPoint;
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