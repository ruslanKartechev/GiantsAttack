﻿using System;
using GameCore.UI;
using SleepDev;
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
        private Transform _flyAtTarget;
            
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
            swipeChecker.SetCorrectDir(stage.evadeDir);
            _currentWeapon = stage.throwTarget.GetComponent<IEnemyThrowWeapon>();
            _currentWeapon.Health.SetMaxHealth(_stage.projectileHealth);
            _currentWeapon.Throwable.SetColliderActive(false);
            _ui = ui;
            _swipeChecker = swipeChecker;
            FailCallback = failCallback;
            SuccessCallback = successCallback;
            if (stage.doWalkToTarget)
                _enemy.Mover.MoveToPoint(stage.moveToPoint, stage.enemyMoveTime, CallPickAndThrow);
            else
                CallPickAndThrow();
        }

        public override void Stop()
        {
            _isStopped = true;
            if (_didSetSlowSlowMoSettings)
            {
                _slowMotionExecutor.RevertSettings();
                StopSwipe();
            }
        }
            
        private void CallPickAndThrow()
        {
            if (_isStopped)
                return;
            _enemy.PickAndThrow(_currentWeapon.Throwable, OnPicked, Throw, false);
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
            _flyAtTarget = new GameObject("throw_at").transform;
            _flyAtTarget.SetParentAndCopy(_player.Point);
            _slowMotionExecutor.BeginSlowMo();
            _currentWeapon.Throwable.FlyTo(_flyAtTarget, _stage.projectileMoveTime, OnFlyEnd, OnCollide );
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
            _flyAtTarget.parent = null;
            _isStopped = true;
            _currentWeapon.Throwable.SetColliderActive(false);
            _slowMotionExecutor.StopSlowMo();
            StopSwipe();
            _player.Mover.Evade(_swipeChecker.LastSwipeDir, OnCorrectEvadeDone, _stage.evadeDistance);
        }

        private void OnWrongSwipe()
        {
            _isStopped = true;
            _slowMotionExecutor.StopSlowMo();
            StopSwipe();
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