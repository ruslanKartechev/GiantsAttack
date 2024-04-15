using System.Collections.Generic;
using SleepDev.Sound;
using UnityEngine;

namespace SleepDev
{
    [DefaultExecutionOrder(1001)]
    public class ExplosiveVehicle : MonoBehaviour
    {
        [SerializeField] private bool _spawnTrail;
        [SerializeField] private VehicleTrail _trail;
        [Space(5)]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private List<ParticleSystem> _onParticles;
        [SerializeField] private List<ParticleSystem> _onParentedParticles;
        [SerializeField] private List<ParticleSystem> _offParticles;
        [SerializeField] private SoundSo _sound;
        [Space(10)]
        [SerializeField] private Transform _defaultDirection;
        [SerializeField] private float _defaultForce;

        public Rigidbody Rb => _rb;
        public Collider Coll => _collider;

        private void Start()
        {
            if(_spawnTrail)
                _trail.Spawn();
        }

        public void Explode()
        {
            PlayParticles();
            _trail?.Off();
            _sound?.Play();
        }
        
        [ContextMenu("ExplodeDefaultDirection")]
        public void ExplodeDefaultDirection()
        {
            Explode(_defaultDirection.forward * _defaultForce);
        }
        
        public void Explode(Vector3 forceVector)
        {
            PlayParticles();
            _collider.enabled = true;
            _rb.isKinematic = false;
            _rb.AddForce(forceVector, ForceMode.Impulse);
            _rb.AddTorque(Vector3.Cross(-forceVector, Vector3.up), ForceMode.Impulse);
            _sound?.Play();
        }
        
        public void Explode(Vector3 forceVector, Vector3 torque)
        {
            PlayParticles();
            _collider.enabled = true;
            _rb.isKinematic = false;
            _rb.AddForce(forceVector, ForceMode.Impulse);
            _rb.AddTorque(torque, ForceMode.Impulse);
            _sound?.Play();
        }

        private void PlayParticles()
        {
            foreach (var pp in _onParticles)
            {
                pp.transform.parent = null;
                pp.gameObject.SetActive(true);
                pp.Play();
            }
            foreach (var pp in _onParentedParticles)
            {
                pp.gameObject.SetActive(true);
                pp.Play();   
            }
            foreach (var pp in _offParticles)
                pp.gameObject.SetActive(false);
        }
        
    }
}