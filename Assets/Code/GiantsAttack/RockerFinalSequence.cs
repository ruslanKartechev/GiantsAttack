using System;
using System.Collections;
using DG.Tweening;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class RockerFinalSequence : LevelFinalSequence
    {
        [SerializeField] private float _playerRotateTime = 1f;
        [SerializeField] private GameObject _rocketCamPointsPrefab;
        [SerializeField] private float _enemyRotTime = .33f;
        [SerializeField] private float _cameraMoveTime;
        [SerializeField] private float _cameraMoveTime2;
        [SerializeField] private float _cameraSendDelay;
        [SerializeField] private Ease _ease1;
        [SerializeField] private Ease _ease2;
        [Space(10)]
        [SerializeField] private float _rocketMoveTime;
        [Space(10)] 
        [SerializeField] private float _endCallbackDelay;        
        [SerializeField] private float _afterEnemyAnimationDelay;
        private Action _endCallback;
        private GameObject _camPointsParent;

#if UNITY_EDITOR
        public override void E_Init()
        { }
#endif

        public override void Begin(Action callback)
        {
            Player.StopAll();
            Player.Mover.StopAll();
            Player.Mover.Loiter();
            Player.Mover.RotateToLook(Enemy.LookAtPoint,_playerRotateTime, () => {});
            _endCallback = callback;
            Enemy.PreKillState();
            Enemy.Mover.RotateToLookAt(Player.Point, _enemyRotTime, OnEnemyRotated);
            Delay(OnEnemyAnimated, _afterEnemyAnimationDelay);
        }

        private void OnEnemyRotated()
        {
        }

        private void OnEnemyAnimated()
        {
            OnRotated();
            // Player.Mover.RotateToLook(Enemy.Point,_helicopterRotTime, OnRotated,true);
        }

        private void OnRotated()
        {
            // Delay(SendCamera, _cameraSendDelay);
            LaunchRockets();
        }
        
        private void LaunchRockets()
        {
            var points = Player.RocketPoints;
            Action callbackSub = OnRocketHit;
            for (var i = 0; i < points.Count; i++)
            {
                var spawnPoint = points[i];
                var atPoint = Enemy.DamagePoints[i];
                var rocket = SpawnRocket(spawnPoint);
                rocket.Fly(atPoint, _rocketMoveTime, callbackSub);
                if (i == 0)
                {
                    callbackSub = null;
                    var camPoints = Instantiate(_rocketCamPointsPrefab, rocket.transform);
                    _camPointsParent = camPoints;
                    StartCoroutine(DelayedCameraSend(camPoints.transform, _cameraSendDelay));
                }
            }
        }

        private IEnumerator DelayedCameraSend(Transform camPointsParent, float delay)
        {
            yield return new WaitForSeconds(delay);
            var p1 = camPointsParent.GetChild(0);
            var p2 = camPointsParent.GetChild(1);
            Camera.SetPoint(p1);
            Camera.Parent(camPointsParent);
            
            var seq = DOTween.Sequence();
            var p3 = camPointsParent.GetChild(2);
            seq.Append(Camera.transform.DOLocalMove(p2.localPosition, _cameraMoveTime).SetEase(_ease1));
            seq.Join(Camera.transform.DOLocalRotate(p2.localRotation.eulerAngles, _cameraMoveTime).SetEase(_ease1));
            seq.Append(Camera.transform.DOLocalMove(p3.localPosition, _cameraMoveTime2).SetEase(_ease2));
            seq.Join(Camera.transform.DOLocalRotate(p3.localRotation.eulerAngles, _cameraMoveTime2).SetEase(_ease2));
            // Camera.MoveToPointLocal(endP, _cameraMoveTime, OnCameraMovedToPos2);
        }

        private void OnCameraMovedToPos2()
        {
            var p2 = _camPointsParent.transform.GetChild(2);
            // var point = new GameObject("temp").transform;
            // point.position = p2.position;
            // point.rotation = Quaternion.LookRotation(Enemy.Point.position - p2.position);
            Camera.MoveToPoint(p2, _cameraMoveTime2, () => {});
        }
        

        private void OnRocketHit()
        {
            CLog.LogRed($"Rocket hit");
            Enemy.Kill();
            Invoke(nameof(RaiseCallback), _endCallbackDelay);
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