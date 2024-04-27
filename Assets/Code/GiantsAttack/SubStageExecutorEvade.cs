using System;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SubStageExecutorEvade : SubStageExecutorBasic
    {
        private IThrowable _throwable;
        private Transform _trackedPoint;
        private bool _doProjectileCollision;
        private bool _startedSlowMo;

        public SubStageExecutorEvade(SubStage stage, IMonster enemy, IHelicopter player,IPlayerMover playerMover, 
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
            _stage.swipeChecker.Off();
        }

        protected override void OnEnemyMoved()
        {
            if (_isStopped) return;
            _throwable = _stage.enemyTarget.GetComponent<IThrowable>();
            _enemy.PickAndThrow(_throwable, OnPicked, Throw, _stage.fromTop);
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
            _player.Aimer.StopAim();
            _player.Shooter.StopShooting();
            _player.Aimer.AimUI.Hide(false);
            
            _throwable.FlyTo(_trackedPoint, _stage.forceVal, OnThrowableFlyEnd, OnThrowableHit);
            if (_stage.doSlowMo)
            {
                _startedSlowMo = true;
                _stage.slowMotionEffect.Begin();
            }
            _ui.EvadeUI.AnimateByDirection(_stage.swipeChecker.CorrectDirection);
            _stage.swipeChecker.On();
            _stage.swipeChecker.OnCorrect = OnCorrect;
            _stage.swipeChecker.OnWrong = OnWrong;
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
        
        private void OnWrong()
        {
            if (_isStopped) return;
            _doProjectileCollision = false;
            StopSlowMo();
            FailAndKillPlayer();
        }

        private void OnCorrect()
        {
            _doProjectileCollision = false;
            _trackedPoint.parent = null;
            _stage.swipeChecker.Off();
            StopSlowMo();
            _ui.EvadeUI.Stop();
            PrintEvent();
            _playerMover.Evade(_stage.swipeChecker.LastSwipeDir, OnEvadeEnd, _stage.evadeDistance);
        }

        private void OnEvadeEnd()
        {
            if (_isStopped) return;
            _throwable.Hide();
            _player.Aimer.BeginAim();
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
            if (_isStopped) return;
            _stage.swipeChecker.Off();
            _throwable.Hide();
            _ui.EvadeUI.Stop();
            _ui.ShootAtTargetUI.Hide();
            KillPlayerAndFail();
        }
    }
}