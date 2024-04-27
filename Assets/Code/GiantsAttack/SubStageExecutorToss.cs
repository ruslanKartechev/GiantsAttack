using System;
using GameCore.UI;
using SleepDev;

namespace GiantsAttack
{
    public class SubStageExecutorToss : SubStageExecutorBasic
    {
        public SubStageExecutorToss(SubStage stage, IMonster enemy, IHelicopter player, IPlayerMover playerMover,
            IGameplayMenu ui, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback) 
            : base(stage, enemy, player, playerMover, ui, counter, delayDelegate, callback, failCallback)
        {
            TryAnimateTarget();
            CallListenersStart();
        }

        protected override void OnEnemyMoved()
        {
            base.OnEnemyMoved();
            var target = _stage.enemyTarget.GetComponent<IThrowable>();
            _enemy.PickAndThrow(target, OnPickUp, OnThrown, 
                _stage.fromTop);
        }
        
        private void OnPickUp()
        {
            if (_isStopped) return;
            _ui.Flash.Play();
            PrintEvent();
        }

        private void OnThrown()
        {
            if (_isStopped) return;
            var target = _stage.enemyTarget.GetComponent<IThrowable>();
            var throwDir = (_stage.forceVal) * (_enemy.Point.forward);
            target.TossTo(throwDir);
            MinusTarget();
            CallListenersCompleted();
            _callback.Invoke();
        }
    }

}