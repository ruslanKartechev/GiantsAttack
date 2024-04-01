using GameCore.Cam;
using GameCore.Core;
using SleepDev;
using SleepDev.SlowMotion;
using UnityEngine;

namespace GiantsAttack
{
    public enum Mode {Evade, ShootDown}

    public class LevelStageThrowAtPlayer : LevelStage
    {
        
        [Header("Debugging")]
        public bool doActivateBoss = true;
        [Space(5)]
        [Header("Mode"), Tooltip("If true will call for evade, else will call shoot")] 
        [SerializeField] private Mode _mode;
        [SerializeField] private bool _doSlowMo;
        [Header("Throwable")]
        [SerializeField] private GameObject _grabTargetGo;
        [SerializeField] private float _projectileHealth = 20;
        [SerializeField] private ShooterSettings _slowMoShooterSettings;
        [Space(5)]
        [Header("Enemy Movement")]
        [SerializeField] private bool _doWalkToTarget;
        [SerializeField] private Transform _moveToPoint;
        [SerializeField] private float _enemyMoveTime;
        [Space(5)]
        [Header("Enemy Throwing")]
        [SerializeField] private float _projectileMoveTime;
        [SerializeField] private Transform _throwAtPoint;
        [SerializeField] private SlowMotionEffectSO _slowMotion;
        [Space(5)]
        [Header("Evasion")]
        [SerializeField] private float _evadeDistance = 10;
        [SerializeField] private CorrectSwipeChecker _correctSwipeChecker;


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
            Player.Mover.Loiter();
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
            _isStopped = true;
            GCon.SlowMotion.SetNormalTime();
            _correctSwipeChecker.Off();
        }

        private void MoveToGrab()
        {
            Enemy.Mover.MoveTo(_moveToPoint, _enemyMoveTime, GrabAndThrow);
        }

        private void GrabAndThrow()
        {
            if (_isStopped)
                return;
            Enemy.PickAndThrow(_enemyWeapon.Throwable, ()=>{} ,Throw);
        }

        private void Throw()
        {
            if (_isStopped)
                return;
            _checkProjectileHit = true;
            _enemyWeapon.Throwable.ThrowAt(_throwAtPoint, _projectileMoveTime, OnThrowableFlyEnd, OnThrowableHit);
            if(_mode == Mode.Evade)
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
            CallCompleted();
        }
        #endregion

        
        #region Evasion
        private void StartEvadeMode()
        {
            CallEvadeUI();
            _correctSwipeChecker.On();
            _correctSwipeChecker.OnCorrect = OnCorrectSwipe;
            _correctSwipeChecker.OnWrong = OnWrongSwipe;
            Player.Aimer.StopAim();
            Player.Shooter.StopShooting();
            _enemyWeapon.Health.SetDamageable(false);
            StartSlowMo();
        }
        
        private void CallEvadeUI()
        {
            UI.EvadeUI.AnimateByDirection(_correctSwipeChecker.CorrectDirection);
        }
        
        private void OnEvadeFail()
        {
            FailAndKillPlayer();
        }
        
        private void OnEvadeSuccess()
        {
            Player.Mover.Loiter();
            Player.Aimer.BeginAim();
            CallCompleted();
        }

        private void OnCorrectSwipe()
        {
            CLog.Log($"[LevelStageEvade] Evade success");
            OnSwipeMade();
            Player.Mover.Evade(_correctSwipeChecker.LastSwipeDir, OnEvadeSuccess, _evadeDistance);
        }

        private void OnWrongSwipe()
        {
            CLog.Log($"[LevelStageEvade] Evade failed");
            OnSwipeMade();
            Player.Mover.Evade(_correctSwipeChecker.LastSwipeDir, OnEvadeFail, _evadeDistance);
        }

        private void OnSwipeMade()
        {
            _correctSwipeChecker.Off();
            UI.EvadeUI.Stop();
            _checkProjectileHit = false;
            StopSlowMo();
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