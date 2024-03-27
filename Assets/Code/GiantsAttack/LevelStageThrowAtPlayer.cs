using GameCore.Cam;
using GameCore.Core;
using SleepDev;
using SleepDev.SlowMotion;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageThrowAtPlayer : LevelStage
    {
        [Header("Debugging")]
        public bool doActivateBoss = true;
        [Space(10)]
        [SerializeField, Header("Mode"), Tooltip("If true will call for evade, else will call shoot")] 
        private bool _doEvade;
        [SerializeField] private bool _doSlowMo;
        [Header("Slow motion")]
        [SerializeField] private float _projectileHealth = 20;
        [SerializeField] private ShooterSettings _slowMoShooterSettings;
        [Header("Throwable GameObject")]
        [SerializeField] private GameObject _grabTargetGo;
        [SerializeField] private Transform _lookAt;
        [Space(10)]
        [Header("Enemy Walking")]
        [SerializeField] private bool _doWalkToTarget;
        [SerializeField] private Transform _moveToPoint;
        [SerializeField] private float _enemyMoveTime;
        [Space(10)]
        [Header("Enemy Throwing")]
        [SerializeField] private float _projectileMoveTime;
        [SerializeField] private Transform _throwAtPoint;
        [SerializeField] private SlowMotionEffectSO _slowMotion;
        [Space(10)]
        [Header("Evasion")]
        [SerializeField] private EDirection2D _evadeDirection;
        [SerializeField] private float _targetSwipeDistance = 10;
        [SerializeField] private SwipeInputTaker _swipeInputTaker;

        private bool _checkProjectileHit;
        private IEnemyThrowWeapon _enemyWeapon;
        private ShooterSettings _shooterSettingsBeforeChange;

        public override void Activate()
        {
            _enemyWeapon = _grabTargetGo.GetComponent<IEnemyThrowWeapon>();
#if UNITY_EDITOR
            UnityEngine.Assertions.Assert.IsNotNull(_enemyWeapon);
#endif
            Player.Mover.StopMovement();
            Player.Aimer.BeginAim();
            Player.Mover.Loiter(_lookAt);
            _swipeInputTaker.enabled = false;
            SubToEnemyKill();
            if (doActivateBoss)
            {
                if (_doWalkToTarget)
                    MoveToGrab();
                else
                    GrabAndThrow();
            }
        }

        public override void Stop()
        {
            GCon.SlowMotion.SetNormalTime();
            
        }

        private void MoveToGrab()
        {
            Enemy.Mover.MoveTo(_moveToPoint, _enemyMoveTime, GrabAndThrow);
        }

        private void GrabAndThrow()
        {
            if (_isStopped)
                return;
            Enemy.PickAndThrow(_enemyWeapon.Throwable, Throw);
        }

        private void Throw()
        {
            if (_isStopped)
                return;
            _checkProjectileHit = true;
            _enemyWeapon.Throwable.ThrowAt(_throwAtPoint.position, _projectileMoveTime, OnThrowableFlyEnd, OnThrowableHit);
            if(_doEvade)
                StartEvadeMode();
            else
                StartShootMode();
        }

        private void ExplodeEnemyWeapon()
        {
            _enemyWeapon.Throwable.Explode();
        }

        private void OnThrowableFlyEnd()
        {
            if (!_checkProjectileHit)
            {
                _enemyWeapon.Throwable.Hide();
                return;
            }
            StopSlowMo();
            ExplodeEnemyWeapon();
            FailAndKillPlayer();
        }

        private void OnThrowableHit(Collider collider)
        {
            if (!_checkProjectileHit)
                return;
            if (collider.TryGetComponent<IHelicopter>(out var player))
            {
                StopSlowMo();
                FailAndKillPlayer();
            }
        }
        
        private void FailAndKillPlayer()
        {
            _enemyWeapon.Throwable.Hide();
            UI.EvadeUI.Stop();
            UI.ShootAtTargetUI.Hide();
            DestroyPlayerAndFail();
        }

        

        #region Shooting at throwable
        private void StartShootMode()
        {
            _enemyWeapon.Health.SetMaxHealth(_projectileHealth);
            _enemyWeapon.Health.SetDamageable(true);
            _enemyWeapon.Health.OnDead += OnThrowableDestroyed;
            UI.ShootAtTargetUI.ShowAndFollow(_enemyWeapon.GameObject.transform);
            Player.Aimer.BeginAim();
            _shooterSettingsBeforeChange = Player.Shooter.Settings;
            Player.Shooter.Settings = _slowMoShooterSettings;
            StartSlowMo();
        }

        private void OnThrowableDestroyed(IDamageable dd)
        {
            _enemyWeapon.Health.OnDead -= OnThrowableDestroyed;
            StopSlowMo();
            UI.ShootAtTargetUI.Hide();
            ExplodeEnemyWeapon();
            Player.Shooter.Settings = _shooterSettingsBeforeChange;
            CameraContainer.Shaker.PlayDefault();
            ResultListener.OnCompleted(this);
        }
        #endregion

        
        #region Evasion
        private void StartEvadeMode()
        {
            CallEvadeUI();
            _swipeInputTaker.enabled = true;
            EnableSwipeInput();
            Player.Aimer.StopAim();
            Player.Shooter.StopShooting();
            _enemyWeapon.Health.SetDamageable(false);
            StartSlowMo();
        }
        
        private void CallEvadeUI()
        {
            UI.EvadeUI.AnimateByDirection(_evadeDirection);
        }
        
        private void OnEvadeFail()
        {
            FailAndKillPlayer();
        }
        
        private void OnEvadeSuccess()
        {
            Player.Mover.Loiter(_lookAt);
            Player.Aimer.BeginAim();
            ResultListener.OnCompleted(this);
        }
        
        private void OnSwipe(EDirection2D direction)
        {
            DisableSwiper();
            UI.EvadeUI.Stop();
            _checkProjectileHit = false;
            StopSlowMo();
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
        #endregion


        private void StartSlowMo()
        {
            if (!_doSlowMo)
                return;
            _slowMotion.Begin();

        }

        private void StopSlowMo()
        {
            if (!_doSlowMo)
                return;
            _slowMotion.Stop();
        }


        
        #if UNITY_EDITOR
        [Space(30)] 
        [Header("Editor")]
        public OnGizmosLineDrawer lineDrawer;
        public Transform rotateTarget;
        public CameraPoint camPoint;

        public void E_RotateToLook()
        {
            if (rotateTarget == null)
            {
                CLog.LogRed("rotateTarget is null");
                return;
            }
            if (_lookAt == null)
            {
                CLog.LogRed("_lookAt is null");
                return;
            }
            rotateTarget.rotation = Quaternion.LookRotation(_lookAt.position - rotateTarget.position);
            UnityEditor.EditorUtility.SetDirty(rotateTarget);
            if (camPoint != null)
            {
                CameraPointMover.SetToPoint(camPoint);
            }
        }
        
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