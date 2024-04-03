using System;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public class SubStageShootDownExecutor : SubStageExecutor
    {
        private IEnemyThrowWeapon _currentWeapon;
        private IMonster _enemy;
        private IHelicopter _player;
        private SubStage _stage;
        private IGameplayMenu _ui;
        private SlowMotionExecutor _slowMotionExecutor; 
        private bool _isStopped;
        private bool _didSetSlowSlowMoSettings;
            
        public Action FailCallback;
        public Action SuccessCallback;
            
        public SubStageShootDownExecutor(SubStage stage, IMonster enemy, IHelicopter player,
            IGameplayMenu ui, SlowMotionExecutor slowMotionExecutor,
            Action successCallback, Action failCallback)
        {
            _stage = stage;
            _enemy = enemy;
            _player = player;
            _slowMotionExecutor = slowMotionExecutor;
            _currentWeapon = stage.throwTarget.GetComponent<IEnemyThrowWeapon>();
            _currentWeapon.Health.SetMaxHealth(_stage.projectileHealth);
            _currentWeapon.Throwable.SetColliderActive(false);
            _ui = ui;
            FailCallback = failCallback;
            SuccessCallback = successCallback;
            if (stage.doWalkToTarget)
                _enemy.Mover.MoveTo(stage.moveToPoint, stage.enemyMoveTime, CallPickAndThrow);
            else
                CallPickAndThrow();
        }
            
        public override void Stop()
        {
            _isStopped = true;
            if (_didSetSlowSlowMoSettings)
            {
                _ui.ShootAtTargetUI.Hide();
                _slowMotionExecutor.RevertSettings();
            }
        }
            
        private void CallPickAndThrow()
        {
            if (_isStopped)
                return;
            _enemy.PickAndThrow(_currentWeapon.Throwable, OnPicked, Throw);
        }
        
        private void OnPicked()
        {
            if (_stage.rotateBeforeThrow == false)
                return;
            _enemy.Mover.RotateToLookAt(_player.Point, _stage.rotateBeforeThrowTime, () => {});
        }

        private void Throw()
        {
            if (_isStopped)
                return;
            _slowMotionExecutor.BeginSlowMo();
            _didSetSlowSlowMoSettings = true;
            _slowMotionExecutor.SetSettings(_player);
            _ui.ShootAtTargetUI.ShowAndFollow(_stage.throwTarget.transform);
            _currentWeapon.Health.OnDead += OnTargetDestroyed;
            _currentWeapon.Throwable.SetColliderActive(true);
            _currentWeapon.Health.SetDamageable(true);
            _currentWeapon.Throwable.ThrowAt(_player.Point, _stage.projectileMoveTime, OnFlyEnd, OnCollide );
        }
            
        private void OnTargetDestroyed(IDamageable obj)
        {
            _isStopped = true;
            _currentWeapon.Throwable.Explode();
            _slowMotionExecutor.StopSlowMo();
            _slowMotionExecutor.RevertSettings();
            _ui.ShootAtTargetUI.Hide();
            obj.OnDead -= OnTargetDestroyed;
            SuccessCallback.Invoke();
        }

        private void OnFlyEnd()
        {
            _currentWeapon.Throwable.Hide();
            _ui.ShootAtTargetUI.Hide();
            if (_isStopped)
                return;
            Fail();
        }

        private void Fail()
        {
            _isStopped = true;
            _slowMotionExecutor.RevertSettings();
            _slowMotionExecutor.StopSlowMo();
            FailCallback.Invoke();
        }
        
        private void OnCollide(Collider collider)
        {
            if (_isStopped)
                return;
            if (collider.TryGetComponent<IHelicopter>(out var player))
            {            
                Fail();
            }
        }
    }
}