using System;
using System.Collections;
using UnityEngine;

namespace GiantsAttack
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _trail;
        [SerializeField] private ParticleSystem _explosion;
        [SerializeField] private GameObject _model;
        
        
        public void Fly(Vector3 endPoint, float time, Action onEnd)
        {
            _trail.gameObject.SetActive(true);
            _trail.Play();
            _model.gameObject.SetActive(true);
            StartCoroutine(Flying(endPoint, time, onEnd));

        }

        private IEnumerator Flying(Vector3 endPoint, float time, Action onEnd)
        {
            var tr = transform;
            var p1 = tr.position;
            var elapsed = 0f;
            while (elapsed <= time)
            {
                tr.position = Vector3.Lerp(p1, endPoint, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }

            tr.position = endPoint;
            _model.gameObject.SetActive(false);
            _explosion.gameObject.SetActive(true);
            _explosion.Play();
            _trail.gameObject.SetActive(false);
            onEnd?.Invoke();
        }
    }
}