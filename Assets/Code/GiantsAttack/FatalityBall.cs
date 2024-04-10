using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class FatalityBall : MonoBehaviour, IFatality
    {
        [SerializeField] private float _addedForce;
        [SerializeField] private float _startDelay = 1f;
        [SerializeField] private SoftBall _softBall;
        [SerializeField] private ParticleSystem _particles;
        private Action _onEnd;
        private bool _completed;
        
        public IHelicopter Player { get; set; }
        public IMonster Enemy { get; set; }
        
        public void Init(IHelicopter helicopter, IMonster monster)
        {
            Player = helicopter;
            Enemy = monster;
            transform.CopyPosRot(monster.Point);
        }

        public void Play(Action onEnd)
        {
            _onEnd = onEnd;
            Invoke(nameof(LaunchBall), _startDelay);
        }

        private void LaunchBall()
        {
            _softBall.CollisionCallback = CollisionHandle;
            _softBall.rb.isKinematic = false;
            _softBall.rb.AddForce(_softBall.transform.forward * _addedForce, ForceMode.VelocityChange);

        }

        private void CollisionHandle(Collider coll)
        {
            if (_completed)
                return;
            _completed = true;
            _particles.Play();
            Enemy.Kill(false);
            _onEnd.Invoke();
        }
    }
}