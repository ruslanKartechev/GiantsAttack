using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class StageListenerExplode : StageListener
    {
        [SerializeField] private bool _onActivated;
        [SerializeField] private ExplosiveVehicle _explosive;


        public override void OnActivated()
        {
            if(_onActivated)
                _explosive.ExplodeDefaultDirection();
        }

        public override void OnStopped()
        {
        }

        public override void OnCompleted()
        {
            if(!_onActivated)
                _explosive.ExplodeDefaultDirection();
        }
    }
}