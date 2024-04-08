using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageBreakBuilding : LevelStage
    {
        [SerializeField] private Transform _enemyPoint;
        [SerializeField] private float _moveTime;
        [SerializeField] private string _animationKey;
        [SerializeField] private BrokenBuilding _building;
        [SerializeField] private float _endDelay;

        public override void Activate()
        {
            SubToEnemyKill();
            Player.Aimer.BeginAim();
            Enemy.Mover.MoveToPoint(_enemyPoint, _moveTime, OnEnemyMoved);
            foreach (var listener in _stageListeners)
            {
                listener.Enemy = Enemy;
                listener.OnActivated();
            }
        }

        private void OnEnemyMoved()
        {
            if (_isStopped)
                return;
            Enemy.PunchStatic(_animationKey, Hit, OnAnimationEnded);
        }

        private void Hit()
        {
            if (_isStopped)
                return;
            _building.Break();
        }

        private void OnAnimationEnded()
        {
            if (_isStopped)
                return;
            Delay(Complete, _endDelay);
        }

        public override void Stop()
        {
            _isStopped = true;
            foreach (var listener in _stageListeners)
                listener.OnStopped();
        }

        private void Complete()
        {
            foreach (var listener in _stageListeners)
                listener.OnCompleted();
            CallCompleted();
        }
    }
}