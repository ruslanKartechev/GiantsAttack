using GameCore.Cam;
using GameCore.Core;
using SleepDev;
using SleepDev.SlowMotion;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageThrowAtPlayer : LevelStage
    {
        [Header("Main")] 
        [SerializeField] private ProjectileStageMode _mode;
        [SerializeField] private float _startDelay = 0f;
        [SerializeField] private bool _waitForPosInPosition;
        [SerializeField] private GameObject _throwable;
        [SerializeField] private bool _doAnimateThrowable;
        [SerializeField] private float _animateThrowableTime;
        [Header("Throwing")]
        [SerializeField] private float _projectileMoveTime;
        [SerializeField] private Transform _throwAtPoint;
        [SerializeField] private bool _doSlowMo;
        [SerializeField] private SlowMotionEffectSO _slowMotion;
        [Header("Enemy Movement")]
        [SerializeField] private bool _doMoveEnemy;
        [SerializeField] private float _enemyMoveTime;
        [SerializeField] private Transform _moveToPoint;
        [Header("PlayerMovement")]
        [SerializeField] private bool _doMovePlayer;
        [SerializeField] private float _playerMoveTime;
        [SerializeField] private Transform _moveToPointPlayer;
        [Header("Evasion")]
        [SerializeField] private float _evadeDistance = 10;
        [SerializeField] private CorrectSwipeChecker _correctSwipeChecker;
        [Header("ShootDown")]
        [SerializeField] private float _projectileHealth = 20;
        [SerializeField] private ShooterSettings _slowMoShooterSettings;
        
        private bool _playerInPos;
        private bool _enemyInPos;
        private bool _didThrow;
        private bool _doProjectileCollision;
        
        private IEnemyThrowWeapon _enemyWeapon;
        private ShooterSettings _shooterSettingsBeforeChange;

        public override void Activate()
        {
            _enemyWeapon = _throwable.GetComponent<IEnemyThrowWeapon>();
#if UNITY_EDITOR
            UnityEngine.Assertions.Assert.IsNotNull(_enemyWeapon);
#endif
            Player.Mover.StopMovement();
            Player.Aimer.BeginAim();
            SubToEnemyKill();
            if (_startDelay > 0)
                Delay(Begin, _startDelay);
            else
                Begin();
        }
        
        public override void Stop()
        {
            _isStopped = true;
            GCon.SlowMotion.SetNormalTime();
            _correctSwipeChecker.Off();
        }

        private void Begin()
        {
            if (_doAnimateThrowable)
            {
                Delay(() => { _enemyWeapon.AnimateMove(OnWeaponAnimateMoved); }, _animateThrowableTime);
            }
            if (_doMoveEnemy)
                MoveEnemy();
            else
                OnEnemyMoved();
            
            if (_doMovePlayer)
                MovePlayer();
            else
            {
                _playerInPos = true;
                Player.Mover.Loiter();
            }
        }

        private void OnWeaponAnimateMoved()
        {
            _enemyWeapon.StopAnimate();
        }
        
        private void MoveEnemy()
        {
            Enemy.Mover.MoveTo(_moveToPoint, _enemyMoveTime, OnEnemyMoved);
        }


        private void MovePlayer()
        {
            Player.Mover.MoveTo(_moveToPointPlayer, _playerMoveTime, null, OnPlayerMoved);    
        }
        
        private void OnPlayerMoved()
        {
            CLog.Log("[StageThrow] OnPlayerMoved");
            Player.Mover.Loiter();
            _playerInPos = true;
            if (_waitForPosInPosition && _enemyInPos && !_didThrow)
            {
                GrabAndThrow();
            }
        }
        
        private void OnEnemyMoved()
        {
            CLog.Log("[StageThrow] OnEnemyMoved");
            _enemyInPos = true;
            if (_waitForPosInPosition && !_playerInPos)
            {
                CLog.Log("[StageThrow] Player not yet in pos");
                return;
            }
            GrabAndThrow();    
        }
        
        private void GrabAndThrow()
        {
            CLog.Log("[StageThrow] GrabAndThrow");
            if (_isStopped)
                return;
            _didThrow = true;
            Enemy.PickAndThrow(_enemyWeapon.Throwable, ()=>{} ,Throw);
        }

        private void Throw()
        {
            if (_isStopped)
                return;
            _doProjectileCollision = true;
            _enemyWeapon.Throwable.ThrowAt(_throwAtPoint, _projectileMoveTime, OnThrowableFlyEnd, OnThrowableHit);
            if(_mode == ProjectileStageMode.Evade)
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
            if (!_doProjectileCollision)
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
            if (!_doProjectileCollision)
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
            _doProjectileCollision = false;
            _correctSwipeChecker.Off();
            UI.EvadeUI.Stop();
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