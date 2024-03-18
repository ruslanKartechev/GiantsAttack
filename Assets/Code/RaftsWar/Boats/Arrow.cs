using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class Arrow : MonoBehaviour, IArrow
    {
        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private ParticleSystem _trail;
        [SerializeField] private ParticleSystem _waterHitParticles;
        [SerializeField] private Collider _collider;
        private DamageDealer _damageDealer;
        private Team _currentTeam;

        public GameObject Go => gameObject;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            transform.parent = null;
            _collider.enabled = false;
            _trail.transform.parent = transform;
            _waterHitParticles.transform.parent = transform;
            _currentTeam = null;
        }

        public void SetView(UnitViewSettings viewSettings)
        {
            for (var i = 0; i < viewSettings.projectileMaterials.Count; i++)
                viewSettings.projectileMaterials[i].Set(_renderers[i]);
        }

        public void Launch(float speed, ITarget target, DamageDealer damageDealer)
        {
            gameObject.SetActive(true);
            _collider.enabled = true;
            _damageDealer = damageDealer;
            StartCoroutine(Flying(speed, 
                target.DamagePointsProvider.GetRandomTarget().position));
            _trail.gameObject.SetActive(true);
            _trail.Play();
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
            var rot1 = Quaternion.LookRotation(p2 - p1);
            var rot2 = Quaternion.LookRotation(p3 - p2);
            while (t <= 1f)
            {
                var p = Bezier.GetPosition(p1, p2, p3, t);
                tr.position = p;
                tr.rotation = Quaternion.Lerp(rot1, rot2, t);
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            tr.position = p3;
            yield return null;
            // TryHit();
            HitAndHide();
        }

        private void TryHit()
        {
            var p2 = transform.position;
            var p1 = new Vector3(p2.x, p2.y + 10f, p2.z);
            if (Physics.Raycast(p1, Vector3.down, out var hit, 200f, GlobalConfig.DamageableMask))
            {
                if (hit.collider.TryGetComponent<ITarget>(out var target))
                {
                    if (target.Damageable != null)
                    {
                        _trail.gameObject.SetActive(false);
                        _damageDealer.DealDamage(target.Damageable);
                        target.ArrowStuckTarget.StuckArrow(this);
                        return;
                    }
                    // CLog.LogRed($"[Arrow] ***** NO DAMAGEEABLE {hit.collider.gameObject.name}");
                }
            }
            // else
                // CLog.LogRed($"[Arrow] ***** NO RAYCAST");
            HitAndHide();
        }

        private void HitAndHide()
        {
            _trail.gameObject.SetActive(false);
            PlayWaterParticles();
            Hide();
                
        }

        private void PlayWaterParticles()
        {
            _waterHitParticles.transform.parent = transform.parent;
            _waterHitParticles.transform.position = transform.position;
            _waterHitParticles.gameObject.SetActive(true);
            _waterHitParticles.Play();
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
                    target.ArrowStuckTarget.StuckArrow(this);
                    _trail.gameObject.SetActive(false);
                    _collider.enabled = false;
                    if(_damageDealer.target.IsDead)
                        Hide();
                    _damageDealer = null;
                }
            }
        }
    }
}