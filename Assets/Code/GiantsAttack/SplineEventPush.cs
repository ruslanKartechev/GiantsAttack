using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SplineEventPush : SplineEvent
    {
        [SerializeField] private bool _moveBeforePush;
        [SerializeField] private SimpleForwardMover _mover;
        [SerializeField] private ExplosiveVehicle _explodingVehicle;
        [SerializeField] private Transform _pushDir;
        [SerializeField] private float _pushForce;
        
        public override void Activate()
        {
            _explodingVehicle.gameObject.SetActive(true);
            if (_moveBeforePush)
                _mover.Move(Explode);
            else
                Explode();
        }

        private void Explode()
        {
            _explodingVehicle.Explode(_pushDir.forward * _pushForce);
        }
        
        
    }
}