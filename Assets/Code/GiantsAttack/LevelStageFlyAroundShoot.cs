using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageFlyAroundShoot : LevelStage
    {
        [SerializeField] private Transform _lookAt;
        [SerializeField] private CircularPathBuilder _circularPathBuilder;
        [SerializeField] private MoveOnCircleArgs _moveOnCircleArgs;
        [Space(10)]
        [SerializeField] private SlowMotionExecutor _slowMotionExecutor;
        [SerializeField] private CorrectSwipeChecker _correctSwipeChecker;
        [SerializeField] private List<SubStage> _subStages;
        private int _stageInd;
        private SubStageExecutor _currentExecutor;

        public override void Activate()
        {
            CLog.LogWhite($"[FlyAroundStage] Activated");
            SubToEnemyKill();
            ActivateCurrentStage();
        }

        public override void Stop()
        {
            _isStopped = true;
            _currentExecutor.Stop();
        }

        private void ActivateCurrentStage()
        {
            if (_isStopped)
                return;
            Player.Aimer.BeginAim();
            Player.Mover.BeginMovingOnCircle(_circularPathBuilder.Path, _lookAt, _moveOnCircleArgs);
            
            var stage = _subStages[_stageInd];
            if (stage.mode == ProjectileStageMode.Evade)
            {
                _currentExecutor = new SubStageEvadeExecutor(stage, Enemy, Player, _correctSwipeChecker, UI,
                    _slowMotionExecutor, OnSubStageSuccess, OnSubStageFailed);
            }
            else
            {
                _currentExecutor = new SubStageShootDownExecutor(stage, Enemy, Player, UI,
                    _slowMotionExecutor, OnSubStageSuccess, OnSubStageFailed);
            }
        }

        private void OnSubStageFailed()
        {
            _isStopped = true;
            DestroyPlayerAndFail();
        }

        private void OnSubStageSuccess()
        {
            CLog.LogWhite($"[LevelStageFlyAroundShoot] SubStage passed");
            _stageInd++;
            if (_stageInd >= _subStages.Count)
            {
                CompleteStage();
                return;
            }
            ActivateCurrentStage();
        }

        private void CompleteStage()
        {
            _isStopped = true;
            CallCompleted();
        }
    }
}