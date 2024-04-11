using UnityEngine;

namespace GiantsAttack
{
    public class StageListenerTankShoot : StageListener
    {
        [SerializeField] private bool _doRotate = true;
        [SerializeField] private float _delayForOnStart = 0f;
        [SerializeField] private TankShooter _tankShooter;
        [SerializeField] private Transform _shootAt;
        
        public override void OnActivated()
        {
            DelayNoReturn(() =>
            {
                if(_doRotate)
                    _tankShooter.ShootOnceInDir(_shootAt);
                else
                    _tankShooter.ShootOnce();
            }, _delayForOnStart);
        }

        public override void OnStopped()
        {
        }

        public override void OnCompleted()
        {
        }
    }
}