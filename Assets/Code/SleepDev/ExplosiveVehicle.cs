using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class ExplosiveVehicle : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private List<ParticleSystem> _onParticles;
        [SerializeField] private List<ParticleSystem> _offParticles;
        [Space(10)]
        [SerializeField] private Transform _defaultDirection;
        [SerializeField] private float _defaultForce;

        public void Explode()
        {
            PlayParticles();
        }
        
        public void ExplodeDefaultDirection()
        {
            Explode(_defaultDirection.forward * _defaultForce);
        }
        
        public void Explode(Vector3 forceVector)
        {
            PlayParticles();
            _collider.enabled = true;
            _collider.isTrigger = false;
            _rb.isKinematic = false;
            _rb.AddForce(forceVector, ForceMode.Impulse);
            _rb.AddTorque(Vector3.Cross(-forceVector, Vector3.up), ForceMode.Impulse);
        }

        private void PlayParticles()
        {
            foreach (var pp in _onParticles)
            {
                pp.gameObject.SetActive(true);
                pp.Play();
            }
            foreach (var pp in _offParticles)
            {
                pp.gameObject.SetActive(false);
            }
        }
        
    }
}