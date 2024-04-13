﻿using System;
using GameCore.UI;

namespace GiantsAttack
{
    public class SubStageExecutorLegKick : SubStageExecutorBasic
    {
        public SubStageExecutorLegKick(SubStage stage, IMonster enemy, IHelicopter player, IPlayerMover playerMover,
            IGameplayMenu ui, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback) 
            : base(stage, enemy, player, playerMover, ui, counter, delayDelegate, callback, failCallback)
        {
            TryAnimateTarget();
            CallListenersStart();
        }

        protected override void OnEnemyMoved()
        {
            _enemy.PunchStatic(_stage.enemyAnimation, LegKick, OnAnimCompleted);
        }
        
        private void LegKick()
        {
            if (_isStopped) return;
            var target = _stage.enemyTarget.GetComponent<AnimatedVehicleBase>();
            target.StopMovement();
            target.ExplodeDefaultDirection();
            MinusTarget();
        }

        private void OnAnimCompleted()
        {
            if (_isStopped) return;
            CallListenersCompleted();
            Complete();
        }
    }
}