using System;
using System.Collections.Generic;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageBreakingCity : LevelStage
    {
        [SerializeField] private List<SubStage> _substages;
        private SubStageExecutor _executor;
        private int _index;
        private IDestroyedTargetsCounter _destroyedTargetsCounter;
        
        public override void Activate()
        {
            Player.Aimer.BeginAim();
            _destroyedTargetsCounter = new DestroyedTargetsCounter(UI, _substages.Count, FailStage);
            SubToEnemyKill();
            ExecuteCurrentSubstage();
        }

        public override void Stop()
        {
            _isStopped = true;
            _executor.Stop();
        }

        private bool NextStage()
        {
            CLog.LogGreen($"[LevelStage] index of sub stage {_index}");
            if (_index >= _substages.Count-1)
                return false;
            _index++;
            Delay(() =>
            {
                ExecuteCurrentSubstage();
            }, _substages[_index].delayBeforeStart);
            return true;
        }

        private void ExecuteCurrentSubstage()
        {
            _executor = new SubStageExecutor(_substages[_index], Enemy, UI, _destroyedTargetsCounter, OnStageCompleted);
        }
        
        private void OnStageCompleted()
        {
            CLog.Log($"On Stage completed");
            if (NextStage())
                return;
            FailStage();
        }

        private void WinStage()
        {
            //    
        }

        private void FailStage()
        {
            // fail basically
        }


        protected enum ActionType
        {
            LegKick, PickAndToss, BreakBuilding,
        }
        
        [System.Serializable]
        protected class SubStage
        {
            public ActionType actionType;
            public float delayBeforeStart;
            [Space(10)]
            public Transform enemyMoveToPoint;
            public bool doMoveEnemy = true;
            public float enemyMoveTime = 1f;
            [Space(10)]
            public GameObject enemyTarget;
            public string enemyAnimation = "Punch";
            public float force;
        }

        protected class SubStageExecutor
        {
            private SubStage _stage;
            private IMonster _enemy;
            private IGameplayMenu _ui;
            private Action _callback;
            private bool _stopped;
            private IDestroyedTargetsCounter _counter;
            
            public SubStageExecutor(SubStage subStage, IMonster enemy, IGameplayMenu ui, IDestroyedTargetsCounter counter, Action completedCallback)
            {
                _stage = subStage;
                _enemy = enemy;
                _ui = ui;
                _callback = completedCallback;
                _counter = counter;
                CLog.Log($"Started executing stage {_stage.actionType.ToString()}");
                if(_stage.doMoveEnemy)
                    enemy.Mover.MoveToPoint(_stage.enemyMoveToPoint, _stage.enemyMoveTime, OnMovedToPoint);
                else
                    OnMovedToPoint();
            }

            public void Stop()
            {
                _stopped = true;
            }

            private void OnMovedToPoint()
            {
                if (_stopped) return;
                if (_stage.actionType is ActionType.PickAndToss)
                {
                    var target = _stage.enemyTarget.GetComponent<IEnemyThrowWeapon>();
                    _enemy.PickAndThrow(target.Throwable, OnPickUp, OnThrown, false);        
                }
                else
                {
                    _enemy.PunchStatic(_stage.enemyAnimation, OnHit, OnAnimCompleted);
                }
            }

            /// <summary>
            /// Enemy animation completed,
            /// </summary>
            private void OnAnimCompleted()
            {
                if (_stopped) return;
 
                _callback.Invoke();
            }
            
            private void OnHit()
            {
                if (_stopped) return;
                switch (_stage.actionType)
                {
                    case ActionType.BreakBuilding:
                        BreakBuilding();
                        break;
                    case ActionType.LegKick:
                        LegKick();
                        break;
                }
            }
            
            private void BreakBuilding()
            {
                CLog.Log($"[SubStage] Action: Break building");
                var target = _stage.enemyTarget.GetComponent<IBrokenBuilding>();
                target.Break();
                MinusTarget();
            }

            private void LegKick()
            {
                CLog.Log($"[SubStage] Action: Leg kick");
                var target = _stage.enemyTarget.GetComponent<AnimatedVehicleBase>();
                target.ExplodeDefaultDirection();
                MinusTarget();
            }

            private void MinusTarget()
            {
                CLog.Log($"[SubStage] Action: Minus target");
                _counter.MinusOne(true);
            }
            
            private void OnPickUp()
            {
                if (_stopped) return;
                CLog.Log($"[SubStage] Action: On Pickup");
            }

            private void OnThrown()
            {
                if (_stopped) return;
                CLog.Log($"[SubStage] Action: Thrown");

                var target = _stage.enemyTarget.GetComponent<IEnemyThrowWeapon>();
                var throwDir = (_stage.force) * (_enemy.Point.forward);
                target.Throwable.TossTo(throwDir);
                MinusTarget();
                OnAnimCompleted();
            }
            
        }
    }
    
}