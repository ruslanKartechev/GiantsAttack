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
            _playerMover.Pause(false);
            _enemy.Punch(_stage.enemyAnimation, OnPunchStarted, OnPunch, OnAnimCompleted);
        }
        
        
        private void OnPunchStarted()
        {
            if (_isStopped) return;
            _ui.EvadeUI.AnimateByDirection(_stage.swipeChecker.CorrectDirection);
            _stage.swipeChecker.OnCorrect = OnCorrect;
            _stage.swipeChecker.OnWrong = OnWrong;
            _stage.swipeChecker.On();
            _player.Aimer.StopAim();
            _player.Shooter.StopShooting();
            if (_stage.doSlowMo)
                _stage.slowMotionEffect.Begin();
        }

        private void OnCorrect()
        {
            if (_isStopped) return;
            Off();
            _hasEvaded = true;
            _playerMover.Evade(_stage.swipeChecker.CorrectDirection, OnEvaded, _stage.evadeDistance);
            _player.Aimer.BeginAim();
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
            _playerMover.Resume();
        }

        private void OnPunch()
        {
            if (_isStopped || _hasEvaded) return;
            KillPlayerAndFail();
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