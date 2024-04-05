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
            Enemy.Mover.MoveTo(_enemyPoint, _moveTime, OnEnemyMoved);
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
            Delay(CallCompleted, _endDelay);
        }

        public override void Stop()
        {
            _isStopped = true;
        }
    }
}