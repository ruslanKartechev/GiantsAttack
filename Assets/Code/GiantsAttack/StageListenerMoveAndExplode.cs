using SleepDev;
using UnityEngine;

namespace GiantsAttack
{ 
    public class StageListenerMoveAndExplode : StageListener
    {  
        [SerializeField] private float _delay;
        [SerializeField] private ExplosiveVehicle _explosive;
        [SerializeField] private SimpleForwardMover _mover;
        
        public override void OnActivated()
        {
            Delay(() => {
                _mover.gameObject.SetActive(true);
                _mover.Move(() =>
                {
                    _explosive.Explode();
                });
            }, _delay);
        }

        public override void OnStopped()
        {
        }

        public override void OnCompleted()
        {
        }
    }
}