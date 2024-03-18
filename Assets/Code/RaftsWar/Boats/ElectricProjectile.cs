using System.Collections;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class ElectricProjectile : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particles;
        private DamageDealer _damageDealer;
        
        public void Launch(float speed, ITarget target, DamageDealer damageDealer)
        {
            _particles.gameObject.SetActive(true);
            _particles.Play();
            _damageDealer = damageDealer;
            gameObject.SetActive(true);
            StartCoroutine(Flying(speed, target.DamagePointsProvider.GetRandomTarget().position));
        }

        private IEnumerator Flying(float speed, Vector3 position)
        {
            var tr = transform;
            var p1 = tr.position;
            var p3 = position;
            var p2 = Vector3.Lerp(p1, p3, .5f) + Vector3.up * 3;
            var length = Bezier.GetLength(p1, p2, p3, 20);
            var time = length / speed;
            var elapsed = Time.deltaTime;
            var t = 0f;
            while (t <= 1f)
            {
                var p = Bezier.GetPosition(p1, p2, p3, t);
                tr.position = p;
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            tr.position = p3;
            yield return null;
            // TryHit();
            Hide();
        }

        private void Hide()
        {
            _particles.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == GlobalConfig.DamageableLayer
                && other.gameObject.CompareTag("Tower")) // skip tower colliders at the bottom of the tower
            {
            }
            if (other.gameObject.TryGetComponent<ITarget>(out var target))
            {
                if (!_damageDealer.CompareTeam(target))
                {
                    StopAllCoroutines();
                    _damageDealer.DealDamage(target.Damageable);
                    _damageDealer = null;
                    Hide();
                }
            }
        }
        
    }
    
}