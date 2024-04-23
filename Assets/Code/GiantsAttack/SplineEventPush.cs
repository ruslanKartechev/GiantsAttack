using UnityEngine;

namespace GiantsAttack
{
    public class SplineEventPush : SplineEvent
    {
        [SerializeField] private bool _moveBeforePush;
        [SerializeField] private AnimatedTarget _animatedVehicle;
        [SerializeField] private Transform _pushDir;
        [SerializeField] private float _pushForce;
        
        public override void Activate()
        {
            _animatedVehicle.gameObject.SetActive(true);
            if (_moveBeforePush)
                _animatedVehicle.Move(Explode);
            else
                Explode();
        }

        private void Explode()
        {
            _animatedVehicle.ExplodeInDirection(_pushDir.forward * _pushForce);
        }
        
        
    }
}