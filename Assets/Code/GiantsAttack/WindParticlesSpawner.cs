using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    [DefaultExecutionOrder(1002)]
    public class WindParticlesSpawner : MonoBehaviour 
    {
        private void Start()
        {
            var win = Instantiate(EnvironmentState.WindParticlesPrefab, transform);
        }
    }
}