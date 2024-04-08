using System;
using System.Collections;
using GameCore.Core;
using GameCore.UI;
using SleepDev;
using SleepDev.Pooling;
using UnityEngine;

namespace GiantsAttack
{
    public class Bullet : MonoExtended, IBullet, IPooledObject<IBullet>
    {
        [SerializeField] private float _maxDistance2;
        [SerializeField] private Transform _movable;
        [SerializeField] private GameObject _model;
        // [SerializeField] private Collider _collider;
        [SerializeField] private ParticleSystem _explosionParticles;
        [SerializeField] private ParticleSystem _trailParticles;
        
        private Coroutine _flying;
        private DamageArgs _damageArgs;
        private IHitCounter _counter;
        private IDamageHitsUI _hitsUI;
        
        public void SetRotation(Quaternion rotation)
        {
            _movable.rotation = rotation;
        }

        public void Launch(Vector3 from, Vector3 direction, float speed, DamageArgs args, 
            IHitCounter counter, IDamageHitsUI hitsUI)
        {
            _explosionParticles.gameObject.SetActive(false);
            _model.gameObject.SetActive(true);
            // _collider.enabled = true;
            _movable.position = from;
            _damageArgs = args;
            _counter = counter;
            _hitsUI = hitsUI;
            StopAllCoroutines();
            gameObject.SetActive(true);
            if (_trailParticles != null)
            {
                _trailParticles.gameObject.SetActive(true);
                _trailParticles.Play();
            }
            _flying = StartCoroutine(Flying(direction, speed));
        }

        private IEnumerator Flying(Vector3 direction, float speed)
        {
            while (true)
            {
                var delta = speed * Time.deltaTime;
                var pos = _movable.position + direction * (delta);
                if (Physics.Raycast(_movable.position, _movable.forward, out var hit, 
                        delta* 2f, GlobalConfig.BulletMask))
                {
                    var d2 = (hit.point - pos).sqrMagnitude;
                    if (d2 <= delta * delta)
                    {
                        ProcessHit(hit.collider);
                    }
                }
                _movable.position = pos;
                yield return null;
            }
        }

        private void OnHit()
        {
            // _collider.enabled = false;
            _model.gameObject.SetActive(false);
            _explosionParticles.gameObject.SetActive(true);
            _explosionParticles.Play();
            StopAllCoroutines();
            Delay(ReturnToPool, 2f);
        }

        private void ProcessHit(Collider other)
        {
            if (other.gameObject.CompareTag(GlobalConfig.PlayerTag))
                return;
            // CLog.Log($"bullet trigger w {other.gameObject.name}");
            if (other.gameObject.TryGetComponent<ITarget>(out var target))
            {
                _damageArgs.point = transform.position;
                _damageArgs.direction = transform.forward;
                target.Damageable.TakeDamage(_damageArgs);
                OnHit();
                _counter.HitsCount++;
                if(target.Damageable.CanDamage)
                    _hitsUI.ShowHit(transform.position, _damageArgs.damage, _damageArgs.isCrit);
            }
            else
            {
                OnHit();
                _counter.MissCount++;
            }
        }

        // Pooling
        public IObjectPool<IBullet> Pool { get; set; }
        public void Parent(Transform parent)
        {
            transform.parent = parent;
        }

        public IBullet Obj => this;
        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void ReturnToPool()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            Pool.ReturnObject(this);
        }

    }
}