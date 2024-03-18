using System;
using System.Collections;
using SleepDev;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;


namespace RaftsWar.Boats
{
    public class CatapultProjectile : MonoBehaviour, ICatapultProjectile
    {
        private static int _eulerInd;
        private static List<Vector3> Eulers = new()
        {
            new Vector3(0,0,0),
            new Vector3(45,0,0),
            new Vector3(0,0,-45),
            new Vector3(-90,0,0),
            new Vector3(0,0,-90),
            new Vector3(-30,0,60)
        };

        private static Vector3 GetEulers()
        {
            if (_eulerInd >= Eulers.Count)
                _eulerInd = 0;
            var vec = Eulers[_eulerInd];
            _eulerInd++;
            return vec;
        }
        
        [SerializeField] private ParticleSystem _particleTrail;
        [SerializeField] private ParticleSystem _particle;
        [SerializeField] private List<BoatSide> _sides;
        private float _damage;
        private ITarget _target;
        
        public virtual void Hide()
        {
            _particleTrail.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        public GameObject Go => gameObject;

        public void Init(Team team)
        {
            gameObject.SetActive(true);
            foreach (var side in _sides)
                side.View.SetView(team.BoatView);
        }

        public void Reset()
        {
            _particle.transform.parent = transform;
            _particleTrail.transform.parent = transform;
            gameObject.SetActive(false);
        }

        public void Launch(Transform fromPoint, ITarget target, float speed, float damage)
        {
            const int bezierSamplesCount = 5;
            var tr = transform;
            tr.parent = null;
            tr.rotation = Quaternion.Euler(GetEulers());
            var p1 = fromPoint.position;
            tr.position = p1;
            var p3 = target.DamagePointsProvider.GetRandomTarget().position;
            var p2 = Vector3.Lerp(p1, p3, .5f) 
                     + Vector3.up * GlobalConfig.CatapultProjectileInflection;
            var time = Bezier.GetLength(p1,p2,p3, bezierSamplesCount) / speed;
            _damage = damage;
            _target = target;
            StartCoroutine(Flying(p1, p2, p3, time));
            TrailOn();
        }

        private void TrailOn()
        {
            _particleTrail.gameObject.SetActive(true);
            _particleTrail.Play();
        }

        private IEnumerator Flying(Vector3 p1, Vector3 p2, Vector3 p3, float time)
        {
            var elapsed = Time.deltaTime;
            var t = elapsed / time;
            var tr = transform;
            while (t <= 1f)
            {
                var p = Bezier.GetPosition(p1, p2, p3, t);
                tr.position = p;
                elapsed += Time.deltaTime;
                t = elapsed / time;
                yield return null;
            }
            Damage();
            PlayParticles();
            Hide();
        }

        private void Damage()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(_target);
            #endif
            _target.Damageable.TakeDamage(new DamageArgs(transform.position, _damage));
        }
        
        private void PlayParticles()
        {
            _particle.transform.parent = null;
            _particle.transform.CopyPosRot(transform);
            _particle.gameObject.SetActive(true);
            _particle.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            
        }
    }
}