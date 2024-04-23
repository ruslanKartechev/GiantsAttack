using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class StageListenerDelayedExplosion : StageListener
    {
        [SerializeField] private float _delay;
        [SerializeField] private ExplosiveVehicle _explosive;

        public override void OnActivated()
        {
            Delay(_explosive.Explode, _delay);
        }

        public override void OnStopped()
        {
        }

        public override void OnCompleted()
        {
        }
    }
}