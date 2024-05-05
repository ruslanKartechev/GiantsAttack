using UnityEngine;

namespace GiantsAttack
{
    public class SplineEventRunawayHumans : SplineEvent
    {
        [SerializeField] private RunawayHumanSpawner _spawner;
        
        public override void Activate()
        {
            _spawner.OnActivated();
        }
    }
}