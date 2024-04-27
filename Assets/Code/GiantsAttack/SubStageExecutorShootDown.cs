using System;
using GameCore.Cam;
using GameCore.Core;
using GameCore.UI;
using SleepDev;
using SleepDev.Vibration;
using UnityEngine;

namespace GiantsAttack
{
    public class SubStageExecutorShootDown : SubStageExecutorBasic
    {
        private ShooterSettings _shooterSettingsBeforeChange;
        private IThrowable _throwable;
        private Transform _trackedPoint;
        private bool _doProjectileCollision;
        private bool _startedSlowMo;

        public SubStageExecutorShootDown(SubStage stage, IMonster enemy, IHelicopter player,IPlayerMover playerMover, 
            IGameplayMenu ui, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback) 
            : base(stage, enemy, player, playerMover, ui, counter, delayDelegate, callback, failCallback)
        {
            CallListenersStart();
            TryAnimateTarget();
        }

        public override void Stop()
        {
            base.Stop();
        }
        
        protected override void OnEnemyMoved()
        {
            if (_isStopped) return;
            _throwable = _stage.enemyTarget.GetComponent<IThrowable>();
            _enemy.PickAndThrow(_throwable, OnPicked ,Throw, _stage.fromTop);
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
            _throwable.FlyTo(_trackedPoint, _stage.forceVal, OnThrowableFlyEnd, OnThrowableHit);
            if (_stage.doSlowMo)
            {
                _startedSlowMo = true;
                _stage.slowMotionEffect.Begin();
            }
            var health = _stage.enemyTarget.GetComponentInChildren<IHealth>();
            health.SetDamageable(true);
            health.OnDead += OnShotDown;
            _ui.ShootAtTargetUI.ShowAndFollow(_stage.enemyTarget.transform);
            _player.Aimer.BeginAim();
            _shooterSettingsBeforeChange = _player.Shooter.Settings;
            var slowMoShooterSettings = new ShooterSettings(_shooterSettingsBeforeChange);
            slowMoShooterSettings.fireDelay /= GlobalConfig.SlowMoFireDelayDiv;
            slowMoShooterSettings.speed *= GlobalConfig.SlowMoBulletSpeedMult;
            _player.Shooter.Settings = slowMoShooterSettings;
            _enemy.Health.SetDamageable(false);
        }

        private void OnShotDown(IDamageable target)
        {
            _trackedPoint.parent = null;
            target.OnDead -= OnShotDown;
            StopSlowMo();
            _ui.ShootAtTargetUI.Hide();
            _throwable.Explode();
            _player.Shooter.Settings = _shooterSettingsBeforeChange;
            CameraContainer.Shaker.PlayDefault();
            _enemy.Health.SetDamageable(true);
            _ui.Flash.Play();
            _ui.BrokenWindowsUI.BreakRandomNumber();
            PrintEvent();
            VibrationManager.VibrManager.PlaySimple();
            Complete();
        }

        private void OnThrowableFlyEnd()
        {
            if (!_doProjectileCollision || _isStopped)
            {
                _throwable.Hide();
                return;
            }
            StopSlowMo();
            _throwable.Explode();
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
            _throwable.Hide();
            _ui.EvadeUI.Stop();
            _ui.ShootAtTargetUI.Hide();
            KillPlayerAndFail();
        }
    }
}