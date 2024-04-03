using System.Collections.Generic;
using UnityEngine;

namespace SleepDev
{
    public class ExplodingVehicle : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private List<ParticleSystem> _onParticles;
        [SerializeField] private List<ParticleSystem> _offParticles;

        public void Explode(Vector3 forceVector)
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

            _collider.enabled = true;
            _rb.isKinematic = false;
            _rb.AddForce(forceVector, ForceMode.VelocityChange);
            _rb.AddTorque(Vector3.Cross(-forceVector, Vector3.up), ForceMode.VelocityChange);
        }
        
    }
}