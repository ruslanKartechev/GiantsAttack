using System;
using GameCore.Cam;
using GameCore.Core;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SubStageExecutorShootDown : SubStageExecutorBasic
    {
        private ShooterSettings _shooterSettingsBeforeChange;
        private IEnemyThrowWeapon _enemyWeapon;
        private Transform _trackedPoint;
        private bool _doProjectileCollision;
        private bool _startedSlowMo;

        public SubStageExecutorShootDown(SubStage stage, IMonster enemy, IHelicopter player,IPlayerMover playerMover, 
            IGameplayMenu ui, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback) 
            : base(stage, enemy, player, playerMover, ui, counter, delayDelegate, callback, failCallback)
        {
            TryAnimateTarget();
            CallListenersStart();
        }

        public override void Stop()
        {
            base.Stop();
        }
        
        protected override void OnEnemyMoved()
        {
            if (_isStopped) return;
            _enemyWeapon = _stage.enemyTarget.GetComponent<IEnemyThrowWeapon>();
            _enemy.PickAndThrow(_enemyWeapon.Throwable, OnPicked ,Throw, _stage.fromTop);
        }

        private void OnPicked()
        {
            if (_isStopped) return;
            _enemy.Mover.RotateToLookAt(_player.Point, _stage.rotationTime, () => {});
        }

        private void Throw()
        {
            if (_isStopped) return;
            _doProjectileCollision = true;
            _trackedPoint = new GameObject("tracked_point").transform;
            _trackedPoint.SetParentAndCopy(_player.Point);
            _enemyWeapon.Throwable.FlyTo(_trackedPoint, _stage.forceVal, OnThrowableFlyEnd, OnThrowableHit);
            if (_stage.doSlowMo)
            {
                _startedSlowMo = true;
                _stage.slowMotionEffect.Begin();
            }
            _enemyWeapon.Health.SetDamageable(true);
            _enemyWeapon.Health.OnDead += OnThrowableDestroyed;
            _ui.ShootAtTargetUI.ShowAndFollow(_enemyWeapon.GameObject.transform);
            _player.Aimer.BeginAim();
            _shooterSettingsBeforeChange = _player.Shooter.Settings;
            var slowMoShooterSettings = new ShooterSettings(_shooterSettingsBeforeChange);
            slowMoShooterSettings.fireDelay /= GlobalConfig.SlowMoFireDelayDiv;
            slowMoShooterSettings.speed *= GlobalConfig.SlowMoBulletSpeedMult;
            _player.Shooter.Settings = slowMoShooterSettings;
            _enemy.Health.SetDamageable(false);
        }

        private void OnThrowableDestroyed(IDamageable obj)
        {
            _trackedPoint.parent = null;
            _enemyWeapon.Health.OnDead -= OnThrowableDestroyed;
            StopSlowMo();
            _ui.ShootAtTargetUI.Hide();
            _enemyWeapon.Throwable.Explode();
            _player.Shooter.Settings = _shooterSettingsBeforeChange;
            CameraContainer.Shaker.PlayDefault();
            _enemy.Health.SetDamageable(true);
            Complete();
        }

        private void OnThrowableFlyEnd()
        {
            if (!_doProjectileCollision || _isStopped)
            {
                _enemyWeapon.Throwable.Hide();
                return;
            }
            StopSlowMo();
            _enemyWeapon.Throwable.Explode();
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
        
        private void StopSlowMo()
        {
            if(_startedSlowMo)
                _stage.slowMotionEffect.Stop();
        }
        
        private void FailAndKillPlayer()
        {
            _enemyWeapon.Throwable.Hide();
            _ui.EvadeUI.Stop();
            _ui.ShootAtTargetUI.Hide();
            KillPlayerAndFail();
        }
    }
}