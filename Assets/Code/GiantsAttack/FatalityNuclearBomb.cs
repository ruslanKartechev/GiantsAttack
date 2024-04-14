using System;
using System.Collections;
using DG.Tweening;
using GameCore.Cam;
using SleepDev;
using SleepDev.Sound;
using UnityEngine;

namespace GiantsAttack
{
    public class FatalityNuclearBomb : MonoBehaviour, IFatality
    {
        [SerializeField] private FatalityType _mType;
        [SerializeField] private float _startDelay;
        [SerializeField] private Transform _bomb;
        [SerializeField] private float _moveTime;
        [SerializeField] private float _bombXAngle;
        [SerializeField] private Ease _moveEase;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private CameraShakeArgs _shakeArgs;
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private Transform _cameraPoint;
        [SerializeField] private float _cameraMoveTime;
        [SerializeField] private SoundSo _explosionSound;

        private Action _callback;

        public IHelicopter Player { get; set; }
        public IMonster Enemy { get; set; }
        public FatalityType Type => _mType;

        public void Init(IHelicopter helicopter, IMonster monster)
        {
            Player = helicopter;
            Enemy = monster;
            transform.CopyPosRot(monster.Point);
        }

        public void Play(Action onEnd)
        {
            _callback = onEnd;
            StartCoroutine(Working());   
        }

        private IEnumerator Working()
        {
            yield return new WaitForSeconds(_startDelay);
            CameraContainer.PlayerCamera.MoveToPoint(_cameraPoint, _cameraMoveTime, () => {});
            _bomb.gameObject.SetActive(true);
            _bomb.DOMove(_endPoint.position, _moveTime).SetEase(_moveEase);
            var endRot = _bomb.rotation * Quaternion.Euler(_bombXAngle, 0f, 0f);
            _bomb.DORotateQuaternion(endRot, _moveTime).SetEase(_moveEase);
            yield return new WaitForSeconds(_moveTime);
            yield return null;
            _bomb.gameObject.SetActive(false);
            CameraContainer.Shaker.Play(_shakeArgs);
            _particle.Play();
            Enemy.Kill(true);
            _explosionSound.Play();
            yield return null;
            _callback.Invoke();
        }

    }
}