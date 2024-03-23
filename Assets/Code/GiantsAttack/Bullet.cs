using System.Collections;
using GameCore.UI;
using SleepDev;
using SleepDev.Pooling;
using UnityEngine;

namespace GiantsAttack
{
    public class Bullet : MonoExtended, IBullet, IPooledObject<IBullet>
    {
        [SerializeField] private Transform _movable;
        [SerializeField] private GameObject _model;
        [SerializeField] private Collider _collider;
        [SerializeField] private ParticleSystem _explosionParticles;
        [SerializeField] private ParticleSystem _trailParticles;
        
        private Coroutine _flying;
        private float _damage;
        private IHitCounter _counter;
        private IDamageHitsUI _hitsUI;
        
        public void SetRotation(Quaternion rotation)
        {
            _movable.rotation = rotation;
        }

        public void Launch(Vector3 from, Vector3 direction, float speed, float damage, 
            IHitCounter counter, IDamageHitsUI hitsUI)
        {
            _explosionParticles.gameObject.SetActive(false);
            _model.gameObject.SetActive(true);
            _collider.enabled = true;
            _movable.position = from;
            _damage = damage;
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
                _movable.position += direction * (speed * Time.deltaTime);
                yield return null;
            }
        }

        private void OnHit()
        {
            _collider.enabled = false;
            _model.gameObject.SetActive(false);
            _explosionParticles.gameObject.SetActive(true);
            _explosionParticles.Play();
            StopAllCoroutines();
            Delay(ReturnToPool, 2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            // CLog.Log($"bullet trigger w {other.gameObject.name}");
            if (other.gameObject.TryGetComponent<ITarget>(out var target))
            {
                target.Damageable.TakeDamage(new DamageArgs(_damage));
                OnHit();
                _counter.HitsCount++;
                _hitsUI.ShowHit(transform.position, _damage);
            }
            else
            {
                OnHit();
                _counter.MissCount++;
            }
        }

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