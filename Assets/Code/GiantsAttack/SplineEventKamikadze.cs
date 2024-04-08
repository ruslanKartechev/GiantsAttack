using System.Collections.Generic;
using GameCore.Cam;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SplineEventKamikadze : SplineEvent
    {
        [SerializeField] private bool _shakeCameraOnHit = true;
        [SerializeField] private float _pushForce;
        [SerializeField] private Transform _movable;
        [SerializeField] private List<Rigidbody> _rigidbodies;
        [SerializeField] private List<ParticleSystem> _particleOnHit;
        [SerializeField] private List<GameObject> _hideOnHit;
        [SerializeField] private List<MonoBehaviour> _disableOnHit;
        [SerializeField] private SimpleForwardMover _mover;
        
        public override void Activate()
        {
            _movable.gameObject.SetActive(true);
            _mover.Move(Hit);
        }

        private void Hit()
        {
            foreach (var rb in _rigidbodies)
            {
                rb.isKinematic = false;
                rb.AddForce(rb.transform.localPosition * _pushForce, ForceMode.VelocityChange);
            }
            foreach (var pp in _particleOnHit)
            {
                pp.gameObject.SetActive(true);
                pp.Play();
            }
            foreach (var mono in _disableOnHit)
                mono.enabled = false;
            foreach (var go in _hideOnHit)
                go.SetActive(false);
            if (_shakeCameraOnHit)
                CameraContainer.Shaker.PlayDefault();
        }
    }
}