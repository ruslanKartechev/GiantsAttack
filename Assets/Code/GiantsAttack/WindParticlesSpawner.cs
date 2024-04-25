using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    [DefaultExecutionOrder(1002)]
    public class WindParticlesSpawner : MonoBehaviour 
    {
        private ParticleSystem _currentParticles;
        
        private void Start()
        {
            _currentParticles = Instantiate(EnvironmentState.WindParticlesPrefab, transform);
        }
    }
}