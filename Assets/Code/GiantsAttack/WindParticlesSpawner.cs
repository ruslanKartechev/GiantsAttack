using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    [DefaultExecutionOrder(1002)]
    public class WindParticlesSpawner : MonoBehaviour 
    {
        [SerializeField] private List<ParticleSystem> _particlesPrefabs;
        private ParticleSystem _currentParticles;
        
        private void Start()
        {
            var env = EnvironmentState.CurrentIndex;
            if (env >= _particlesPrefabs.Count)
            {
                CLog.LogRed($"env >= _particlesPrefabs.Count");
                env = (byte)(_particlesPrefabs.Count - 1);
            }
            _currentParticles = Instantiate(_particlesPrefabs[env], transform);
        }
    }
}