using System;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class RockerFinalSequence : LevelFinalSequence
    {
        [SerializeField] private float _helicopterRotTime = .1f;
        [SerializeField] private float _cameraMoveTime;
        [SerializeField] private float _cameraOffset;
        [SerializeField] private float  _cameraSendDelay;
        [Space(10)]
        [SerializeField] private float _rocketMoveTime;
        [SerializeField] private float _rocketOffset;
        [Space(10)] 
        [SerializeField] private float _callDelay;        
        private Action _endCallback;
        
        
        public override void Begin(Action callback)
        {
            _endCallback = callback;
            Enemy.PreKillState();
            Player.Mover.RotateToLook(Enemy.Point,_helicopterRotTime, OnRotated,true);
        }

        private void OnRotated()
        {
            Delay(SendCamera, _cameraSendDelay);
            LaunchRockets();
        }
        
        private void LaunchRockets()
        {
            var points = Player.RocketPoints;
            Action callbackSub = OnRocketHit;
            for (var i = 0; i < points.Count; i++)
            {
                var p = points[i];
                var atPoint = Enemy.DamagePoints[i];
                var rocket = SpawnRocket(p);
                var vec = atPoint.position - p.position;
                var length = vec.magnitude;
                length -= _rocketOffset;
                var endPoint = p.position + vec.normalized * length;
                rocket.Fly(endPoint, _rocketMoveTime, callbackSub);
                if (i == 0)
                    callbackSub = null;
            }
        }
        
        private void SendCamera()
        {
            var enemyPos = Enemy.LookAtPoint.position;
            Camera.StopMoving();
            Camera.Unparent();
            var vec = enemyPos - Camera.transform.position;
            vec.y = 0f;
            var length = vec.magnitude;
            length -= _cameraOffset;
            var endCamPoint = Camera.transform.position + vec.normalized * length;
            var camPoint = new GameObject("gg").transform;
            camPoint.position = endCamPoint;
            camPoint.rotation = Quaternion.LookRotation(enemyPos - endCamPoint);
            Camera.MoveToPoint(camPoint, _cameraMoveTime, () => {});
        }

        private void OnRocketHit()
        {
            CLog.LogRed($"Rocket hit");
            Enemy.Kill();
            Invoke(nameof(RaiseCallback), _callDelay);
        }

        private void RaiseCallback()
        {
            _endCallback.Invoke();
        }

        private Rocket SpawnRocket(Transform point)
        {
            var rocket = GCon.GOFactory.Spawn<Rocket>("rocket");
            rocket.transform.SetPositionAndRotation(point.position, point.rotation);
            return rocket;
        }
        
    }
}