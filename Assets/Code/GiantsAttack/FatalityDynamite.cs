using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameCore.Cam;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class FatalityDynamite : MonoBehaviour, IFatality
    {
        [SerializeField] private FatalityType _mType;
        [SerializeField] private float _scaleTime;
        [SerializeField] private Ease _scaleEase;
        [SerializeField] private float _explosionDelay;
        [SerializeField] private float _nextDynamiteDelay;
        [SerializeField] private List<Dynamite> _dynamites;
        [SerializeField] private CameraShakeArgs _shakeArgs;
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
            foreach (var d in _dynamites)
            {
                d.ScaleUp(_scaleTime, _scaleEase);
                yield return new WaitForSeconds(_nextDynamiteDelay);
            }
            yield return new WaitForSeconds(_explosionDelay);
            foreach (var d in _dynamites)
                d.Explode();
            CameraContainer.Shaker.Play(_shakeArgs);
            Enemy.Kill(true);
            yield return null;
            _callback.Invoke();
        }
        
    }
}