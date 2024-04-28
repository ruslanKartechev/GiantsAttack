using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class DefeatedBehaviour : MonoBehaviour, IDefeatedBehaviour
    {
        [SerializeField] private string _animKey;
        [SerializeField] private Animator _animator;
        [SerializeField] private MonsterAnimEventReceiver _eventReceiver;
        [SerializeField] private ParticleSystem _fallDownParticles;
        [SerializeField] private List<ParticleSystem> _playParticles;
        private Action _onPlayed; 
        
        public void Play(Action onPlayed)
        {
            _onPlayed = onPlayed;
            _eventReceiver.EOnStoodUp += EOnStoodUp;
            _eventReceiver.EOnJumpDown += OnJumpDown;
            for (var i = 0; i < _animator.parameterCount; i++)
            {
                var p = _animator.GetParameter(i);
                _animator.ResetTrigger(p.name);
            }
            _animator.Play(_animKey);
            foreach (var particle in _playParticles)
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            }
        }

        private void OnJumpDown()
        {
            _eventReceiver.EOnJumpDown -= OnJumpDown;
            _fallDownParticles.gameObject.SetActive(true);
            _fallDownParticles.Play();
        }

        private void EOnStoodUp()
        {
            _eventReceiver.EOnStoodUp -= EOnStoodUp;
            _onPlayed?.Invoke();
        }
    }
}