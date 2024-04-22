using System;
using System.Collections.Generic;
using GameCore.Core;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageHavok : LevelStage
    {
        [SerializeField] private List<SubStage> _substages;
        private IDestroyedTargetsCounter _counter;
        private SubStageExecutor _executor;
        private int _index;
        private int _totalCount;
        
        public CityDestroyUI CityUI { get; set; }

        public int GetTotalCount()
        {
            if (_totalCount > 0)
                return _totalCount;
            _totalCount = 0;
            foreach (var ss in _substages)
                _totalCount += ss.targetsCount;
            return _totalCount;
        }
        
        public override void Activate()
        {
            CityUI = ((IGameplayMenu)GCon.UIFactory.GetGameplayMenu()).CityDestroyUI;
            GetTotalCount();
            CityUI.SetCount(_totalCount, _totalCount);
            CityUI.Show(() => {});
            foreach (var listener in _stageListeners)
                listener.OnActivated();
            Player.Aimer.BeginAim();
            _counter = new DestroyedTargetsCounter(CityUI, GetTotalCount(), Fail);
            SubToEnemyKill();
            Delay(() =>
            {
                ExecuteCurrentSubstage();
            }, _substages[_index].delayBeforeStart);
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
            if (_isStopped) return;
            _executor = _substages[_index].GetExecutor(Enemy, player:Player, playerMover:PlayerMover,
                menu:UI,_counter,DelayNoReturn,OnSubStageCompleted, OnSubStageFailed);
        }
        
        private void OnSubStageCompleted()
        {
            CLog.Log($"On Stage completed");
            if (_isStopped) return;
            if (NextStage())
                return;
            Fail();
        }

        private void OnSubStageFailed()
        {
            CLog.Log($"[{nameof(LevelStageBreakBuilding)}] A Substance {_index} was failed");
            Fail();
        }

        protected override void OnEnemyKilled(IMonster enemy)
        {
            base.OnEnemyKilled(enemy);    
        }
        
        private void Fail()
        {
            CLog.LogWhite($"[{nameof(LevelStageHavok)}] FAILED");
            Stop();
            ResultListener.OnStageFail(this);
        }
        
        
#if UNITY_EDITOR
        [Space(10)] [Header("Editor")] 
        public Transform e_startPoint;
        public float e_speed;

        [ContextMenu("E_CalculateSpeed")]
        public void E_CalculateSpeed()
        {
            if (e_startPoint == null)
            {
                CLog.Log($"Start point is null");
                return;
            }
            var startPoint = e_startPoint;
            foreach (var ss in _substages)
            {
                if(!ss.doMoveEnemy)
                    continue;
                if (ss.enemyMoveToPoint == null)
                {
                    CLog.Log("Target point is null");
                    break;
                }
                var distance = (startPoint.position - ss.enemyMoveToPoint.position).XZDistance();
                var time = distance / e_speed;
                ss.enemyMoveTime = time;
                startPoint = ss.enemyMoveToPoint;
                UnityEditor.EditorUtility.SetDirty(ss);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        

        //
        // protected class dSubStageExecutor
        // {
        //     private SubStage _stage;
        //     private IMonster _enemy;
        //     private Action _callback;
        //     private bool _stopped;
        //     private IDestroyedTargetsCounter _counter;
        //     private Action<Action, float> _delayDelegate;
        //     
        //     public dSubStageExecutor(SubStage subStage, IMonster enemy, IDestroyedTargetsCounter counter,Action<Action, float> delayDelegate, Action completedCallback)
        //     {
        //         _stage = subStage;
        //         _enemy = enemy;
        //         _callback = completedCallback;
        //         _counter = counter;
        //         _delayDelegate = delayDelegate;
        //         CLog.Log($"Started executing stage {_stage.actionType.ToString()}");
        //         if (_stage.delayBeforeStart > 0)
        //             _delayDelegate.Invoke(Begin, _stage.delayBeforeStart);
        //         else
        //             Begin();
        //         if (_stage.doAnimateTarget)
        //         {
        //             if(_stage.delayBeforeAnimateTarget > 0)
        //                 _delayDelegate.Invoke(AnimateTarget, _stage.delayBeforeAnimateTarget);
        //             else
        //                 AnimateTarget();
        //         }
        //     }
        //     
        //     public void Stop()
        //     {
        //         _stopped = true;
        //     }
        //     
        //     private void Begin()
        //     {
        //         if (_stopped) return;
        //         if(_stage.doMoveEnemy)
        //             _enemy.Mover.MoveToPoint(_stage.enemyMoveToPoint, _stage.enemyMoveTime, OnMovedToPoint);
        //         else
        //             OnMovedToPoint();
        //     }
        //     
        //     private void AnimateTarget()
        //     {
        //         if (_stopped) return;
        //         switch (_stage.targetAnimationType)
        //         {
        //             case AnimationType.Animate:
        //             {
        //                 _stage.enemyTarget.GetComponent<AnimatedVehicleBase>().AnimateMove();
        //             } break;
        //             case AnimationType.Move:
        //             {
        //                 _stage.enemyTarget.GetComponent<AnimatedVehicleBase>().Move();
        //             } break;
        //         }
        //     }
        //
        //     private void OnMovedToPoint()
        //     {
        //         if (_stopped) return;
        //         if (_stage.actionType is ActionType.Toss)
        //         {
        //             var target = _stage.enemyTarget.GetComponent<IEnemyThrowWeapon>();
        //             _enemy.PickAndThrow(target.Throwable, OnPickUp, OnThrown, 
        //                 _stage.fromTop);        
        //         }
        //         else
        //         {
        //             _enemy.PunchStatic(_stage.enemyAnimation, OnHit, OnAnimCompleted);
        //         }
        //     }
        //
        //     /// <summary>
        //     /// Enemy animation completed,
        //     /// </summary>
        //     private void OnAnimCompleted()
        //     {
        //         if (_stopped) return;
        //         _callback.Invoke();
        //     }
        //     
        //     private void OnHit()
        //     {
        //         if (_stopped) return;
        //         switch (_stage.actionType)
        //         {
        //             case ActionType.BreakBuilding:
        //                 BreakBuilding();
        //                 break;
        //             case ActionType.LegKick:
        //                 LegKick();
        //                 break;
        //         }
        //     }
        //     
        //     private void BreakBuilding()
        //     {
        //         CLog.Log($"[SubStage] Action: Break building");
        //         var target = _stage.enemyTarget.GetComponent<IBrokenBuilding>();
        //         target.Break();
        //         MinusTarget();
        //     }
        //
        //     private void LegKick()
        //     {
        //         CLog.Log($"[SubStage] Action: Leg kick");
        //         var target = _stage.enemyTarget.GetComponent<AnimatedVehicleBase>();
        //         target.ExplodeDefaultDirection();
        //         MinusTarget();
        //     }
        //
        //     private void MinusTarget()
        //     {
        //         CLog.Log($"[SubStage] Action: Minus target");
        //         _counter.MinusOne(true);
        //     }
        //     
        //     private void OnPickUp()
        //     {
        //         if (_stopped) return;
        //         CLog.Log($"[SubStage] Action: On Pickup");
        //     }
        //
        //     private void OnThrown()
        //     {
        //         if (_stopped) return;
        //         CLog.Log($"[SubStage] Action: Thrown");
        //
        //         var target = _stage.enemyTarget.GetComponent<IEnemyThrowWeapon>();
        //         var throwDir = (_stage.forceVal) * (_enemy.Point.forward);
        //         target.Throwable.TossTo(throwDir);
        //         MinusTarget();
        //         OnAnimCompleted();
        //     }
        //     
        // }
        //
    }
    
}