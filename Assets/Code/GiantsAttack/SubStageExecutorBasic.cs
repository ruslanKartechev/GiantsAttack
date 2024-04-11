using System;
using GameCore.UI;

namespace GiantsAttack
{
    public class SubStageExecutorBasic : SubStageExecutor
    {
        protected IMonster _enemy;
        protected IHelicopter _player;
        protected IGameplayMenu _ui;
        protected IPlayerMover _playerMover;
        protected IDestroyedTargetsCounter _counter;
        protected Action<Action, float> _delayDelegate;
        protected SubStage _stage;
        protected Action _callback;
        protected Action _failCallback;
        protected bool _isStopped;
        protected AnimatedVehicleBase _animatedVehicle;
        
        public SubStageExecutorBasic(SubStage stage, 
            IMonster enemy, IHelicopter player, IPlayerMover playerMover,
            IGameplayMenu ui, 
            IDestroyedTargetsCounter counter,
            Action<Action, float> delayDelegate, 
            Action callback, Action failCallback)
        {
            _stage = stage;
            _enemy = enemy;
            _player = player;
            _ui = ui;
            _playerMover = playerMover;
            _counter = counter;
            _delayDelegate = delayDelegate;
            _callback = callback;
            _failCallback = failCallback;
            if (stage.doMoveEnemy)
                _enemy.Mover.MoveToPoint(_stage.enemyMoveToPoint, _stage.enemyMoveTime, OnEnemyMoved);
            else
                OnEnemyMoved();
        }

        protected virtual void TryAnimateTarget()
        {
            if (_stage.doAnimateTarget)
            {
                _animatedVehicle = _stage.enemyTarget.GetComponent<AnimatedVehicleBase>();
                if(_stage.delayBeforeAnimateTarget > 0)
                    _delayDelegate.Invoke(AnimateTarget, _stage.delayBeforeAnimateTarget);
                else
                    AnimateTarget();
            }
        }

        protected virtual void AnimateTarget()
        {
            switch (_stage.targetAnimationType)
            {
                case AnimationType.Animate:
                    _animatedVehicle.AnimateMove();
                    break;
                case AnimationType.Move:
                    _animatedVehicle.Move();
                    break;
            }
        }
        
        public override void Stop()
        {
            _isStopped = true;
        }

        protected virtual void OnEnemyMoved()
        {
        }

        protected virtual void MinusTarget()
        {
            _counter.MinusOne(true);
        }

        protected virtual void Complete()
        {
            _callback.Invoke();
        }
        
        protected virtual void DestroyPlayerAndFail()
        {
            _player.Kill();
            _failCallback.Invoke();
        }

        protected virtual void CallListenersStart()
        {
            foreach (var listener in _stage.stageListeners)
            {
                listener.Enemy = _enemy;
                listener.OnActivated();
            }
        }

        protected virtual void CallListenersCompleted()
        {
            foreach (var listener in _stage.stageListeners)
            {
                listener.Enemy = _enemy;
                listener.OnCompleted();
            }
        }        
    }
}