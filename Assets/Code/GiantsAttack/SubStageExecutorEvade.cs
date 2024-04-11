﻿using System;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SubStageExecutorEvade : SubStageExecutorBasic
    {
        private IEnemyThrowWeapon _enemyWeapon;
        private Transform _trackedPoint;
        private bool _doProjectileCollision;
        private bool _startedSlowMo;

        public SubStageExecutorEvade(SubStage stage, IMonster enemy, IHelicopter player,IPlayerMover playerMover, 
            IGameplayMenu ui, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback) 
            : base(stage, enemy, player, playerMover, ui, counter, delayDelegate, callback, failCallback)
        {
            TryAnimateTarget();
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
            _stage.swipeChecker.On();
            _stage.swipeChecker.OnCorrect = OnCorrect;
            _stage.swipeChecker.OnWrong = OnWrong;
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
        
        private void OnWrong()
        {
            StopSlowMo();
            FailAndKillPlayer();
        }

        private void OnCorrect()
        {
            _trackedPoint.parent = null;
            StopSlowMo();
            _playerMover.Evade(_stage.swipeChecker.LastSwipeDir, OnEvadeEnd, _stage.evadeDistance);
        }

        private void OnEvadeEnd()
        {
            CLog.Log($"[SubStageExecutor] Evaded...");
            Complete();
            _playerMover.Resume();
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
            DestroyPlayerAndFail();
        }
    }
}