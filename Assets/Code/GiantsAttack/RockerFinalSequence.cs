using System;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class RockerFinalSequence : LevelFinalSequence
    {
        [SerializeField] private float _flyTime;
        private Action _endCallback;
        
        public override void Begin(Action callback)
        {
            _endCallback = callback;
            var points = Player.RocketPoints;
            Action callbackSub = OnRocketHit;
            Camera.StopMoving();
            Camera.Unparent();
            Camera.SetPoint(Player.RocketCamPoint);
            var i = 0;
            foreach (var p in points)
            {
                var rocket = SpawnRocket(p);
                var v = Enemy.Point.position - p.position;
                var length = Vector3.Dot(p.forward, v);
                rocket.Fly(p.position + p.forward * length, _flyTime, callbackSub);
                if (i == 0)
                {
                    callbackSub = null;
                    Camera.FollowWithOffset(rocket.transform);
                }
                i++;
            }
        }

        private void OnRocketHit()
        {
            CLog.LogRed($"Rocket hit");
            Enemy.Kill();
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