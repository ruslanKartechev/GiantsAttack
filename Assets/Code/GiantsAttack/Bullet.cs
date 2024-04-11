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
        private const float MaxFlyTime = 20f;
        [SerializeField] private Transform _movable;
        // [SerializeField] private GameObject _model;
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

        public void Scale(float scale)
        {
            transform.localScale = new Vector3(scale,scale,scale);
        }

        public void Launch(Vector3 from, Vector3 direction, float speed, DamageArgs args, 
            IHitCounter counter, IDamageHitsUI hitsUI)
        {
            _damageArgs = args;
            _counter = counter;
            _hitsUI = hitsUI;
            _movable.position = from;
            _explosionParticles.gameObject.SetActive(false);
            gameObject.SetActive(true);
            _trailParticles.gameObject.SetActive(true);
            _trailParticles.Play();
            _flying = StartCoroutine(Flying(direction, speed));
        }

        public void LaunchBlank(Vector3 from, Vector3 direction, float speed)
        {
            _movable.position = from;
            _explosionParticles.gameObject.SetActive(false);
            gameObject.SetActive(true);
            _trailParticles.gameObject.SetActive(true);
            _trailParticles.Play();
            _flying = StartCoroutine(FlyingBlank(direction, speed));
        }

        private IEnumerator FlyingBlank(Vector3 direction, float speed)
        {
            var time = MaxFlyTime;
            while (time > 0)
            {
                var delta = speed * Time.deltaTime;
                var pos = _movable.position + direction * (delta);
                if (Physics.Raycast(_movable.position, _movable.forward, out var hit, 
                        delta * 2f, GlobalConfig.BulletMask))
                {
                    var d2 = (hit.point - pos).sqrMagnitude;
                    // if (d2 <= delta * delta)
                        // OnHit();
                }
                _movable.position = pos;
                time -= Time.unscaledDeltaTime;
                yield return null;
            }
            gameObject.SetActive(false);
        }
        
        private IEnumerator Flying(Vector3 direction, float speed)
        {
            var time = MaxFlyTime;
            while (time > 0)
            {
                var delta = speed * Time.deltaTime;
                var pos = _movable.position + direction * (delta);
                if (Physics.Raycast(_movable.position, _movable.forward, out var hit, 
                        delta * 2f, GlobalConfig.BulletMask))
                {
                    var d2 = (hit.point - pos).sqrMagnitude;
                    if (d2 <= delta * delta)
                    {
                        ProcessHit(hit.collider);
                    }
                }
                _movable.position = pos;
                time -= Time.unscaledDeltaTime;
                yield return null;
            }
            gameObject.SetActive(false);
        }

        private void OnHit()
        {
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