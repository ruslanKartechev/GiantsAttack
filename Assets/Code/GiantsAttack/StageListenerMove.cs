using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class StageListenerMove : StageListener
    {
        [SerializeField] private AnimatedTarget _animatedVehicle;
        [SerializeField] private bool _hideOnMoveEnd;
        [SerializeField] private float _delay;

        public override void OnActivated()
        {
            if (_delay > 0)
                Delay(Move, _delay);
            else
                Move();
            
            void Move()
            {
                _animatedVehicle.gameObject.SetActive(true);
                if (_hideOnMoveEnd)
                    _animatedVehicle.Move(() => {_animatedVehicle.gameObject.SetActive(false);});   
                else
                    _animatedVehicle.Move();
            }
        }

        public override void OnStopped()
        {
            // _animatedVehicle.StopMovement();
        }

        public override void OnCompleted()
        {
        }
    }
}