using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class GodzillaDefeatedBehaviour : MonoBehaviour, IDefeatedBehaviour
    {
        [SerializeField] private string _animKey;
        [SerializeField] private Animator _animator;
        [SerializeField] private List<ParticleSystem> _offParticles;
        [SerializeField] private List<ParticleSystem> _playParticles;
        
        public void Play(Action onPlayed)
        {
            foreach (var pp in _offParticles)
                pp.gameObject.SetActive(false);
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
            onPlayed.Invoke();
        }
        
    }
}