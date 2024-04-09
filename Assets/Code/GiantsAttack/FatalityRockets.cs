using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCore.Cam;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class FatalityRockets : MonoBehaviour, IFatality
    {
        [SerializeField] private FatalityType _mType;
        [SerializeField] private GameObject _rocketCamPoints;
        [SerializeField] private List<Transform> _rocketPoints;
        [SerializeField] private float _cameraMoveTime;
        [SerializeField] private float _cameraMoveTime2;
        [SerializeField] private float _cameraSendDelay;
        [SerializeField] private Ease _ease1;
        [SerializeField] private Ease _ease2;
        [Space(10)]
        [SerializeField] private float _rocketMoveTime;
        private Action _callback;

        public IHelicopter Player { get; set; }
        public IMonster Enemy { get; set; }
        public FatalityType Type => _mType;

        public void Init(IHelicopter helicopter, IMonster monster)
        {
            Player = helicopter;
            Enemy = monster;
        }

        public void Play(Action onEnd)
        {
            _callback = onEnd;
            transform.SetParentAndCopy(Player.Point);
            LaunchRockets();
        }
        
        private void LaunchRockets()
        {
            Action callbackSub = OnRocketHit;
            for (var i = 0; i < _rocketPoints.Count; i++)
            {
                var spawnPoint = _rocketPoints[i];
                var atPoint = Enemy.DamagePoints[i];
                var rocket = SpawnRocket(spawnPoint);
                rocket.Fly(atPoint, _rocketMoveTime, callbackSub);
                if (i == 0)
                {
                    callbackSub = null;
                    _rocketCamPoints.transform.SetParentAndCopy(rocket.transform);
                    StartCoroutine(DelayedCameraSend(_rocketCamPoints.transform, _cameraSendDelay));
                }
            }
        }
        
        private Rocket SpawnRocket(Transform point)
        {
            var rocket = GCon.GOFactory.Spawn<Rocket>("rocket");
            rocket.transform.SetPositionAndRotation(point.position, point.rotation);
            return rocket;
        }
        
        private void OnRocketHit()
        {
            CLog.LogRed($"Rocket hit");
            Enemy.Kill();
            _callback.Invoke();
        }

        private IEnumerator DelayedCameraSend(Transform camPointsParent, float delay)
        {
            yield return new WaitForSeconds(delay);
            var p1 = camPointsParent.GetChild(0);
            var p2 = camPointsParent.GetChild(1);
            var camera = CameraContainer.PlayerCamera;
            camera.SetPoint(p1);
            camera.Parent(camPointsParent);
            
            var seq = DOTween.Sequence();
            var p3 = camPointsParent.GetChild(2);
            seq.Append(camera.Transform.DOLocalMove(p2.localPosition, _cameraMoveTime).SetEase(_ease1));
            seq.Join(camera.Transform.DOLocalRotate(p2.localRotation.eulerAngles, _cameraMoveTime).SetEase(_ease1));
            seq.Append(camera.Transform.DOLocalMove(p3.localPosition, _cameraMoveTime2).SetEase(_ease2));
            seq.Join(camera.Transform.DOLocalRotate(p3.localRotation.eulerAngles, _cameraMoveTime2).SetEase(_ease2));
            // Camera.MoveToPointLocal(endP, _cameraMoveTime, OnCameraMovedToPos2);
        }
    }
}