using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using SleepDev.Utils;
using UnityEngine;

namespace GiantsAttack
{
    public class FatalityC4 : MonoBehaviour, IFatality
    {
        [System.Serializable]
        public class MoveArgs
        {
            public Vector2 offsetLimits;
            public Vector2 moveTime;
            public float punchScale;
            public float punchScaleTime;
            public Ease moveEase;

            public void Play(Transform target)
            {
                var pos = target.position;
                var endPos = pos;
                pos.y += offsetLimits.RandomInVec();
                target.position = pos;
                var seq = DOTween.Sequence();
                seq.Append(target.DOMove(endPos, moveTime.RandomInVec()).SetEase(moveEase));
                seq.Append(target.DOPunchScale(punchScale * Vector3.one, punchScaleTime));
            }
        }

        [SerializeField] private float _moveDuration;
        [SerializeField] private float _nextFuseDelay;
        [SerializeField] private float _explosionDelay;
        [SerializeField] private float _soulsDelay;
        
        [SerializeField] private List<GameObject> _movables;
        [SerializeField] private MoveArgs _moveArgs;
        [SerializeField] private ParticleSystem _soulsParticles;
        
        private Action _onEnd;

        public IHelicopter Player { get; set; }
        
        public IMonster Enemy { get; set; }
        
        public void Init(IHelicopter helicopter, IMonster monster)
        {
            Player = helicopter;
            Enemy = monster;
        }

        public void Play(Action onEnd)
        {
            _onEnd = onEnd;
            transform.CopyPosRot(Enemy.Point);
            StartCoroutine(Working());
        }

        private IEnumerator Working()
        {
            foreach (var mb in _movables)
                _moveArgs.Play(mb.transform);
            yield return new WaitForSeconds(_moveDuration);
            foreach (var mb in _movables)
            {
                mb.transform.GetChild(1).gameObject.SetActive(true);
                yield return new WaitForSeconds(_nextFuseDelay);
            }
            yield return null;
            foreach (var mb in _movables)
            {
                mb.transform.GetChild(0).gameObject.SetActive(false);
                mb.transform.GetChild(1).gameObject.SetActive(false);
                mb.transform.GetChild(2).gameObject.SetActive(true);
            }
            Enemy.Kill(true);
            yield return new WaitForSeconds(_soulsDelay);
            _soulsParticles.gameObject.SetActive(true);
            _soulsParticles.Play();
            yield return null;
            _onEnd.Invoke();
        }
        
        
    }
}