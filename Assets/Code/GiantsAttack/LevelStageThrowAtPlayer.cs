using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageThrowAtPlayer : LevelStage
    {
        [SerializeField] private GameObject _grabTargetGo;
        [SerializeField] private Transform _moveToPoint;
        [SerializeField] private float _moveTime;
        private IThrowable _grabTarget;
        
        
        public override void Activate()
        {
            _grabTarget = _grabTargetGo.GetComponent<IThrowable>();
#if UNITY_EDITOR
            UnityEngine.Assertions.Assert.IsNotNull(_grabTarget);
#endif
            Player.Mover.StopMovement();
            Enemy.Mover.MoveTo(_moveToPoint, _moveTime, OnMovedToGrab);
        }

        private void OnMovedToGrab()
        {
            Enemy.GrabTarget(_grabTarget, OnGrabbed);
        }

        private void OnGrabbed()
        {
            // Enemy.ThrowAt(_grabTarget, );
        }

        public override void Stop()
        {
            
        }
    }
}