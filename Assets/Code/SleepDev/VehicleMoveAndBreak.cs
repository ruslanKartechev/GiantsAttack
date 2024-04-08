using UnityEngine;

namespace SleepDev
{
    public class VehicleMoveAndBreak : MonoBehaviour
    {
        [SerializeField] private bool _autoStart;
        [SerializeField] private SimpleForwardMover _mover;
        [SerializeField] private float _pushForceUp;
        [SerializeField] private ExplosiveVehicle _explodingVehicle;
        private Coroutine _working;

        private void Start()
        {
            if(_autoStart)
                Begin();
        }

        public void Begin()
        {
            _mover.Move(OnMoveEnd);
        }

        private void OnMoveEnd()
        {
            var force = Vector3.up * _pushForceUp;
            _explodingVehicle.Explode(force);
        }
        
    }
}