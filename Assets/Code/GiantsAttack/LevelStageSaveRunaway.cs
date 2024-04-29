using System.Collections;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageSaveRunaway : LevelStage
    {
        [SerializeField] private bool _simpleStartAll;
        [SerializeField] private float _runawayInitDelay;
        [SerializeField] private float _runawayMoveDelay;
        [Space(10)]
        [SerializeField] private float _runawayStartSpeed;
        [Space(10)]
        [SerializeField] private float _runawayAccelerationDelay;
        [SerializeField] private float _runawayAcceleratedSpeed;
        [SerializeField] private float _runawayAccelerationTime;
        [Space(10)]
        [SerializeField] private float _enemyToStartMoveTime;
        [SerializeField] private float _enemyToStartMoveDelay;
        [SerializeField] private string _enemyWalkAnim = "Walk";
        [Space(10)]
        [SerializeField] private float _enemyStartSpeed;
        [SerializeField] private float _enemyStartSplineT;
        [Space(10)] 
        [SerializeField] private float _failSplinePercent;
        [Space(10)]
        [SerializeField] private Transform _enemyStartPoint;
        [SerializeField] private SplineMover _enemyMover;
        [SerializeField] private SplineMover _runawayMover;        
        [SerializeField] private GameObject _runawayGo;
        private IRunaway _runaway;
        
        public override void Activate()
        {
            _runaway = _runawayGo.GetComponent<IRunaway>();
            Player.Aimer.BeginAim();
            if (_simpleStartAll)
            {
                _runaway.Init();
                _runaway.Mover.MoveAccelerated();
                _enemyMover.MoveAccelerated();
                Enemy.Animate(_enemyWalkAnim, true);
            }
            else
            {
                Delay(InitRunaway, _runawayInitDelay);
                Delay(StartMovingRunaway, _runawayMoveDelay);
                Delay(MoveEnemyToStart, _enemyToStartMoveDelay);
            }
            SubToEnemyKill();
            StartCoroutine(FailPercentPolling());
        }

        public override void Stop()
        {
            _isStopped = true;
        }

        private void InitRunaway()
        {
            _runaway.Init();
        }

        private void MoveEnemyToStart()
        {
            if(_enemyStartPoint != null)
                Enemy.Mover.MoveToPoint(_enemyStartPoint, _enemyToStartMoveTime, OnEnemyMovedToStart);
            else
                OnEnemyMovedToStart();
        }

        private void OnEnemyMovedToStart()
        {
            Enemy.Animate("Walk", false);
            _enemyMover.Speed = _enemyStartSpeed;
            _enemyMover.InterpolationT = _enemyStartSplineT;
            _enemyMover.MoveNow();
            // Debug.Break();
        }

        private void StartMovingRunaway()
        {
            _runaway.Mover.Speed = _runawayStartSpeed;
            _runaway.BeginMoving();
            Delay(AccelerateRunaway, _runawayAccelerationDelay);
        }

        private void AccelerateRunaway()
        {
            _runaway.Mover.ChangeSpeed(_runawayAcceleratedSpeed, _runawayAccelerationTime);
        }

        protected override void OnEnemyKilled(IMonster enemy)
        {
            _isStopped = true;
            PlayerMover.Pause(false);
            _enemyMover.Stop();
            StopAllCoroutines();
            base.OnEnemyKilled(enemy);
        }

        private IEnumerator FailPercentPolling()
        {
            while (!_isStopped)
            {
                if (_runawayMover.InterpolationT >= _failSplinePercent)
                {
                    _runaway.Stop();
                    _enemyMover.Stop();
                    UnsubFromEnemy();
                    ResultListener.OnStageFail(this);
                    StopAllCoroutines();
                }
                yield return null;
            }
        }
        
        #if UNITY_EDITOR
        [ContextMenu("E_CalculateTimeForEnemy")]
        public void E_CalculateTimeForEnemy()
        {
            if (_enemyMover == null || _enemyStartPoint == null)
                return;
            var dist = (_enemyMover.transform.position - _enemyStartPoint.position).magnitude;
            var time = dist / _enemyStartSpeed;
            _enemyToStartMoveTime = time;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}