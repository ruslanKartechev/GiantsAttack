using System;
using GameCore.UI;
using SleepDev;
using SleepDev.Vibration;

namespace GiantsAttack
{
    public class SubStageExecutorBreakBuilding : SubStageExecutorBasic
    {
        public SubStageExecutorBreakBuilding(SubStage stage, IMonster enemy, IHelicopter player,IPlayerMover playerMover,
            IGameplayMenu ui, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback) 
            : base(stage, enemy, player, playerMover, ui, counter, delayDelegate, callback, failCallback)
        {
            TryAnimateTarget();
            CallListenersStart();
        }

        protected override void OnEnemyMoved()
        {
            if (_isStopped) return;
            _enemy.PunchStatic(_stage.enemyAnimation, BreakBuilding, CallCompleted);
        }
        
        private void BreakBuilding()
        {
            if (_isStopped) return;
            CLog.Log($"[SubStage] Action: Break building");
            var target = _stage.enemyTarget.GetComponent<IBrokenBuilding>();
            target.Break();
            _ui.Flash.Play();
            VibrationManager.VibrManager.PlaySimple();
            PrintEvent();
            MinusTarget();
        }

        private void CallCompleted()
        {
            if (_isStopped) return;
            CallListenersCompleted();
            Complete();
        }
        
    }
}