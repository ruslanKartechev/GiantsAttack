using System;
using GameCore.UI;

namespace GiantsAttack
{
    public class SubStageExecutorPlayerAttack : SubStageExecutorBasic
    {
        private bool _hasEvaded;

        public SubStageExecutorPlayerAttack(SubStage stage, IMonster enemy, IHelicopter player, IPlayerMover playerMover,
            IGameplayMenu ui, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback) 
            : base(stage, enemy, player, playerMover, ui, counter, delayDelegate, callback, failCallback)
        {
            CallListenersStart();
        }

        protected override void OnEnemyMoved()
        {
            if (_isStopped) return;
            _enemy.Punch(_stage.enemyAnimation, OnPunchStarted, OnPunch, OnAnimCompleted);
        }
        
        
        private void OnPunchStarted()
        {
            if (_isStopped) return;
            _playerMover.Pause(false);
            _ui.EvadeUI.AnimateByDirection(_stage.swipeChecker.CorrectDirection);
            _player.Aimer.StopAim();
            _player.Shooter.StopShooting();
            _stage.swipeChecker.OnCorrect = OnCorrect;
            _stage.swipeChecker.OnWrong = OnWrong;
            _stage.swipeChecker.On();
            if (_stage.doSlowMo)
                _stage.slowMotionEffect.Begin();
        }

        private void OnCorrect()
        {
            if (_isStopped) return;
            Off();
            _hasEvaded = true;
            _playerMover.Evade(_stage.swipeChecker.CorrectDirection, OnEvaded, _stage.evadeDistance);
            // _player.Aimer.BeginAim();
        }
        
        private void OnWrong()
        {
            if (_isStopped) return;
            Off();
            KillPlayerAndFail();
        }
        
        private void OnEvaded()
        {
            if (_isStopped) return;
            if(_stage.skip1)
                _playerMover.ZeroWaitTime();
            if(_stage.skip2)
                _playerMover.SkipToNextPoint();
            _player.Aimer.BeginAim();
            _playerMover.Resume();
        }

        private void OnPunch()
        {
            if (_hasEvaded || _isStopped) return;
            Off();
            KillPlayerAndFail();
            _isStopped = true;
        }

        private void Off()
        {
            if (_stage.doSlowMo)
                _stage.slowMotionEffect.Stop();
            _stage.swipeChecker.Off();
            _ui.EvadeUI.Stop();
        }
        
        private void OnAnimCompleted()
        {
            Complete();
        }
    }
}