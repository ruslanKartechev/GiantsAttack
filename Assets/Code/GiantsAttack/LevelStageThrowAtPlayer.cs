﻿using GameCore.Cam;
using GameCore.Core;
using SleepDev;
using SleepDev.SlowMotion;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageThrowAtPlayer : LevelStage
    {
        public bool doActivateBoss = true;
        [Space(20)]
        [SerializeField, Tooltip("If true will call for evade, else will call shoot")] 
        private bool _doEvade;
        [SerializeField] private float _projectileHealth = 20;
        [SerializeField] private ShooterSettings _slowMoShooterSettings;
        [SerializeField] private bool _doSlowMo;
        [Header("Throwable GameObject")]
        [SerializeField] private GameObject _grabTargetGo;
        [SerializeField] private Transform _lookAt;
        [Space(10)]
        [Header("Walking")]
        [SerializeField] private bool _doWalkToTarget;
        [SerializeField] private Transform _moveToPoint;
        [SerializeField] private float _enemyMoveTime;
        [Space(10)]
        [Header("Throwing")]
        [SerializeField] private float _projectileMoveTime;
        [SerializeField] private Transform _throwAtPoint;
        [SerializeField] private SlowMotionEffectSO _slowMotion;
        [Space(10)]
        [Header("Evasion")]
        [SerializeField] private EDirection2D _evadeDirection;
        [SerializeField] private float _targetSwipeDistance = 10;
        [SerializeField] private SwipeInputTaker _swipeInputTaker;
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
            Enemy.PickAndThrow(_enemyWeapon.Throwable, Throw);
            // Enemy.ThrowAt(_grabTarget, );
        }

        private void Throw()
        {
            _enemyWeapon.Throwable.ThrowAt(_throwAtPoint.position, _projectileMoveTime, HideThrowable, OnThrowableHit);
            if(_doEvade)
                StartEvadeMode();
            else
                StartShootMode();
        }

        private void HideThrowable()
        {
            _enemyWeapon.Throwable.Hide();
        }

        private void OnThrowableHit(Collider collider)
        {
            // CLog.Log($"Throwable Collider with {collider.gameObject.name}");
            if (collider.CompareTag(GlobalConfig.PlayerTag))
            {
                FailAndKillPlayer();
            }
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
            GCon.SlowMotion.Exit(_slowMotion.Effect);
            UI.ShootAtTargetUI.Hide();
            _enemyWeapon.Throwable.Hide();
            Player.Shooter.Settings = _shooterSettingsBeforeChange;
            CameraContainer.Shaker.PlayDefault();
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
            // fail game    
            FailAndKillPlayer();
        }
        
        private void OnEvadeSuccess()
        {
            StopSlowMo();
            Player.Mover.BeginAnimating();
            CompletedCallback.Invoke();
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

        private void FailAndKillPlayer()
        {
            _enemyWeapon.Throwable.Hide();
            UI.EvadeUI.Stop();
            UI.ShootAtTargetUI.Hide();
            DestroyPlayerAndFail();
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