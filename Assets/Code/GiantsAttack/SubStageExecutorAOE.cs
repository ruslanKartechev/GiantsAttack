using System;
using GameCore.Cam;
using GameCore.UI;

namespace GiantsAttack
{
    public class SubStageExecutorAOE : SubStageExecutorBasic
    {
        public SubStageExecutorAOE(SubStage stage, IMonster enemy, IHelicopter player, IPlayerMover playerMover,
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
            _enemy.Jump(true);
            _enemy.AnimEventReceiver.OnJumpDown += OnJumped;
        }
        
        private void OnJumped()
        {
            _enemy.AnimEventReceiver.OnJumpDown -= OnJumped;
            if (_isStopped) return;
            CameraContainer.Shaker.PlayDefault();
            var target = _stage.enemyTarget.GetComponent<AnimatedTarget>();
            target.ExplodeDefaultDirection();
            _counter.Minus( _stage.targetsCount, true);
            CallListenersCompleted();
            _ui.Flash.Play();
            PrintEvent();
            Complete();
        }
    }
}