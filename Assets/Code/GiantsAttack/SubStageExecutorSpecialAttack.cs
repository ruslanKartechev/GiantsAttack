using System;
using GameCore.UI;

namespace GiantsAttack
{
    public class SubStageExecutorSpecialAttack : SubStageExecutorBasic
    {
        private ISpecAttackTarget _target;
        
        public SubStageExecutorSpecialAttack(SubStage stage, IMonster enemy, IHelicopter player, IPlayerMover playerMover,
            IGameplayMenu ui, IDestroyedTargetsCounter counter, 
            Action<Action, float> delayDelegate, Action callback, Action failCallback) 
                : base(stage, enemy, player, playerMover, ui, counter, delayDelegate, callback, failCallback)
        {
            CallListenersStart();
            if (_stage.doMoveEnemy)
            {
                _target = _stage.enemyTarget.GetComponent<ISpecAttackTarget>();
                _target.OnStageBegan();
            }
        }
        
        protected override void OnEnemyMoved()
        {
            if (_isStopped) return;
            if (_stage.doMoveEnemy == false)
            {
                _target = _stage.enemyTarget.GetComponent<ISpecAttackTarget>();
                _target.OnStageBegan();
            }
            _enemy.SpecialAttack(_stage.enemyAnimation, AttackStarted, 
                Attack, OnCompleted);
        }

        private void AttackStarted()
        {
            if (_isStopped) return;
            _target.OnAttackBegan();
        }

        private void Attack()
        {
            if (_isStopped) return;
            _target.OnAttack();
            MinusTarget();
            _ui.Flash.Play();
        }

        private void OnCompleted()
        {
            if (_isStopped) return;
            _target.OnCompleted();
            Complete();
        }
        
    }
}