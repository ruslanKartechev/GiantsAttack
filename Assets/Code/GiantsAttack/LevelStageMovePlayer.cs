using System;
using SleepDev;
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
        [SerializeField] private bool _callEnemyMove = true;
        [SerializeField] private float _enemyDelay;
        [SerializeField] private Transform _enemyMovePoint;
        [SerializeField] private float _enemyMoveTime;
        [SerializeField] private bool _callEnemyRoar;

        public override void Activate()
        {
            SubToEnemyKill();

            if (_allowShooting)
                Player.Aimer.BeginAim();
            else
            {
                Player.Aimer.StopAim();
                Player.Shooter.StopShooting();
            }

            if (_delay > 0)
                Delay(CallMove, _delay);
            else
                CallMove();
            if (_callEnemyMove)
            {
                if (_enemyDelay > 0)
                    Delay(CallMoveEnemy, _enemyDelay);
                else
                    CallMoveEnemy(); 
            }
            else if (_callEnemyRoar)
            {
                Enemy.Roar();
            }
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
                if(_isStopped == false)
                    Enemy.Idle();
            });
        }

        public override void Stop()
        {
            _isStopped = true;
        }

        protected override void OnEnemyKilled(IMonster obj)
        {
            StopAllCoroutines();
            base.OnEnemyKilled(obj);
        }

        private void OnMovementDone()
        {
            CLog.Log($"[StageMove] movement end, calling completed");
            _isStopped = true;
            CallCompleted();
        }
    }
}