using GameCore.Core;
using SleepDev;
using SleepDev.SlowMotion;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageThrowAtPlayer : LevelStage
    {
        [SerializeField] private GameObject _grabTargetGo;
        [Space(10)]
        [SerializeField] private bool _doWalkToTarget;
        [SerializeField] private Transform _moveToPoint;
        [SerializeField] private float _enemyMoveTime;
        [Space(10)]
        [SerializeField] private Transform _throwAtPoint;
        [SerializeField] private float _projectileMoveTime;
        [SerializeField] private SlowMotionEffectSO _slowMotion;
        [Space(10)]
        [SerializeField] private EDirection2D _evadeDirection;
        [SerializeField] private float _targetSwipeDistance = 10;
        [SerializeField] private SwipeInputTaker _swipeInputTaker;
        private IThrowable _throwable;
        
        public override void Activate()
        {
            _throwable = _grabTargetGo.GetComponent<IThrowable>();
#if UNITY_EDITOR
            UnityEngine.Assertions.Assert.IsNotNull(_throwable);
#endif
            Player.Mover.StopMovement();
            Player.Aimer.BeginAim();
            if (_doWalkToTarget)
                MoveToGrab();
            else
                GrabAndThrow();
            _swipeInputTaker.enabled = false;

        }

        public override void Stop()
        {
            GCon.SlowMotion.SetNormalTime();
               
        }
        
        private void OnSwipe(EDirection2D direction)
        {
            DisableSwiper();
            UI.EvadeUI.Stop();
            GCon.SlowMotion.Exit(_slowMotion.Effect);
            if (direction == _evadeDirection)
            {
                CLog.Log($"[LevelStageEvade] Evade success");
                Player.Mover.Evade(direction, OnEvadeSuccess);
                
            }
            else
            {
                CLog.Log($"[LevelStageEvade] Evade failed");
                Player.Mover.Evade(direction, OnEvadeFail);
                
            }
        }

        private void MoveToGrab()
        {
            Enemy.Mover.MoveTo(_moveToPoint, _enemyMoveTime, GrabAndThrow);
        }

        private void GrabAndThrow()
        {
            Enemy.PickAndThrow(_throwable, Throw);
            // Enemy.ThrowAt(_grabTarget, );
        }

        private void Throw()
        {
            _throwable.ThrowAt(_throwAtPoint.position, _projectileMoveTime, HideThrowable, OnThrowableHit);
            CallEvadeUI();
            _swipeInputTaker.enabled = true;
            EnableSwipeInput();
            // Player.Aimer.StopAim();
            // Player.Shooter.StopShooting();
        }

        private void EnableSwipeInput()
        {
            _swipeInputTaker.Refresh();
            _swipeInputTaker.TargetDistance = _targetSwipeDistance;
            _swipeInputTaker.OnSwipeIndirection += OnSwipe;
        }

        private void DisableSwiper()
        {
            _swipeInputTaker.OnSwipeIndirection -= OnSwipe;
            _swipeInputTaker.enabled = false;   
        }
        
        private void HideThrowable()
        {
            _throwable.Hide();
        }

        private void OnThrowableHit(Collider collider)
        {
            CLog.Log($"Collider with {collider.gameObject.name}");
        }

        private void CallEvadeUI()
        {
            UI.EvadeUI.AnimateByDirection(_evadeDirection);
            _slowMotion.Begin();
        }
        
        private void OnEvadeFail()
        {
            // fail game    
        }
        
        private void OnEvadeSuccess()
        {
            // Player.Aimer.StopAim();
            CompletedCallback.Invoke();
        }
        
        
        #if UNITY_EDITOR
        [Space(10)] 
        public OnGizmosLineDrawer lineDrawer;
        private void OnDrawGizmos()
        {
            if (lineDrawer != null
                && _moveToPoint != null
                && _throwAtPoint != null)
            {
                lineDrawer.DrawLine(_moveToPoint.position, _throwAtPoint.position);
            }
        }
#endif
    }
}