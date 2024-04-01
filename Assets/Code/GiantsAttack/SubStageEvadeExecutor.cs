using System;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public class SubStageEvadeExecutor : SubStageExecutor
    {
        private IEnemyThrowWeapon _currentWeapon;
        private IMonster _enemy;
        private IHelicopter _player;
        private SubStage _stage;
        private CorrectSwipeChecker _swipeChecker;
        private IGameplayMenu _ui;
        private SlowMotionExecutor _slowMotionExecutor; 
        private bool _isStopped;
        private bool _didSetSlowSlowMoSettings;
            
        public Action FailCallback;
        public Action SuccessCallback;
            
        public SubStageEvadeExecutor(SubStage stage, IMonster enemy, IHelicopter player,
            CorrectSwipeChecker swipeChecker, IGameplayMenu ui, SlowMotionExecutor slowMotionExecutor,
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
            _swipeChecker = swipeChecker;
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
            if(_didSetSlowSlowMoSettings)
                _slowMotionExecutor.RevertSettings();
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
            _currentWeapon.Throwable.ThrowAt(_player.Point, _stage.projectileMoveTime, OnFlyEnd, OnCollide );
            _player.Aimer.StopAim();
            _player.Shooter.StopShooting();
            _swipeChecker.OnCorrect = OnCorrectSwipe;
            _swipeChecker.OnWrong = OnWrongSwipe;
            _swipeChecker.On();
            _ui.EvadeUI.AnimateByDirection(_swipeChecker.CorrectDirection);
            _currentWeapon.Throwable.SetColliderActive(true);
        }
            
        private void OnCorrectSwipe()
        {
            _isStopped = true;
            _currentWeapon.Throwable.SetColliderActive(false);
            _slowMotionExecutor.StopSlowMo();
            StopSwipe();
            _player.Mover.Evade(_swipeChecker.LastSwipeDir, OnCorrectEvadeDone, _stage.evadeDistance);
        }

        private void OnWrongSwipe()
        {
            _slowMotionExecutor.StopSlowMo();
            _isStopped = true;
            _player.Mover.Evade(_swipeChecker.LastSwipeDir, OnWrongSwipeDone, _stage.evadeDistance);
        }
            
        private void OnCorrectEvadeDone()
        {
            SuccessCallback.Invoke();
        }
            
        private void OnWrongSwipeDone()
        {
            FailCallback.Invoke();
        }

        private void StopSwipe()
        {
            _ui.EvadeUI.Stop();
            _swipeChecker.Off();
        }
            
        private void OnFlyEnd()
        {
            _currentWeapon.Throwable.Hide();
            if (_isStopped)
                return;
            _isStopped = true;
            FailCallback.Invoke();
        }

        private void OnCollide(Collider collider)
        {
            if (_isStopped)
                return;
            if (collider.TryGetComponent<IHelicopter>(out var player))
            {            
                _isStopped = true;
                FailCallback.Invoke();
            }
        }
    }
}