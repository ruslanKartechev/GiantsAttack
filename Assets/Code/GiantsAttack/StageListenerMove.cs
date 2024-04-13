using UnityEngine;

namespace GiantsAttack
{
    public class StageListenerMove : StageListener
    {
        [SerializeField] private AnimatedVehicleBase _animatedVehicle;

        public override void OnActivated()
        {
            _animatedVehicle.Move();
        }

        public override void OnStopped()
        {
            _animatedVehicle.StopMovement();
        }

        public override void OnCompleted()
        {
        }
    }
}