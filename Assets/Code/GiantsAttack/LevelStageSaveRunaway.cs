using System.Collections;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageSaveRunaway : LevelStage
    {
        [SerializeField] private bool _initRunaway;
        [SerializeField] private float _runawayMoveDelay;
        [SerializeField] private float _enemyMoveDelay;
        [Space(10)] 
        [SerializeField] private bool _accelerateRunaway;
        [SerializeField] private bool _accelerateEnemy;
        [Space(10)] 
        [SerializeField, Range(0f,1f)] private float _failSplinePercent;
        [Space(10)]
        [SerializeField] private SplineMover _enemyMover;
        [SerializeField] private SplineMover _runawayMover;        
        [SerializeField] private GameObject _runawayGo;
        private IRunaway _runaway;
        
        public override void Activate()
        {
            foreach (var listener in _stageListeners)
                listener.OnActivated();
            _runaway = _runawayGo.GetComponent<IRunaway>();
            Player.Aimer.BeginAim();
            if(_initRunaway)
                _runaway.Init();
            Delay(MoveEnemy, _enemyMoveDelay);
            Delay(MoveRunaway, _runawayMoveDelay);
            SubToEnemyKill();
            StartCoroutine(FailPercentPolling());
        }

        public override void Stop()
        {
            _isStopped = true;
        }
        
        private void MoveEnemy()
        {
            Enemy.Animate("Walk", true);
            if(_accelerateEnemy)
                _enemyMover.MoveAccelerated();
            else
                _enemyMover.MoveNow();
        }

        private void MoveRunaway()
        {
            if(_accelerateRunaway)
                _runawayMover.MoveAccelerated();
            else
                _runawayMover.MoveNow();
            _runaway.BeginMoving();
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
        
    }
}