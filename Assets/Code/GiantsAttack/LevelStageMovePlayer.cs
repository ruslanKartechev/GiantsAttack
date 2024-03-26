using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageMovePlayer : LevelStage
    {
        [SerializeField] private bool _allowShooting;
        [Header("Player Movement")]
        [SerializeField] private float _delay;    
        [SerializeField] private Transform _playerMoveToPoint;
        [SerializeField] private float _playerMoveTime;
        [SerializeField] private AnimationCurve _moveCurve;
        [Header("Enemy movement")]
        [SerializeField] private float _enemyDelay;
        [SerializeField] private Transform _enemyMovePoint;
        [SerializeField] private float _enemyMoveTime;


        public override void Activate()
        {
            if (_allowShooting)
                Player.Aimer.BeginAim();
            else
                Player.Aimer.StopAim();

            if (_delay > 0)
                Delay(CallMove, _delay);
            else
                CallMove(); 
            if (_enemyDelay > 0)
                Delay(CallMoveEnemy, _enemyDelay);
            else
                CallMoveEnemy(); 
        }

        private void CallMove()
        {
            Player.Mover.StopLoiter(true);
            Player.Mover.MoveTo(_playerMoveToPoint, _playerMoveTime, _moveCurve, OnMovementDone);
        }

        private void CallMoveEnemy()
        {
            Enemy.Mover.MoveTo(_enemyMovePoint, _enemyMoveTime, () =>
            {
                Enemy.Idle();
            });
        }

        public override void Stop()
        {
        }

        private void OnMovementDone()
        {
            CompletedCallback.Invoke();
        }
    }
}