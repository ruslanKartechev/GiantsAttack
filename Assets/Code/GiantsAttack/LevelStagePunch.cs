using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStagePunch : LevelStage
    {
        [SerializeField] private string _animKey;
        [SerializeField] private bool _waitForBoth;
        [SerializeField] private bool _resumePlayerMoveRightAfterEvasion;
        [Space(10)]
        [SerializeField] private bool _doMoveEnemy;
        [SerializeField] private float _enemyMoveTime;
        [SerializeField] private Transform _enemyPoint;
        [Header("After evasion movement Enemy")] 
        [SerializeField] private bool _enemyMoveAfterEvaded;
        [SerializeField] private float _enemyAfterEvasionMoveTime;
        [SerializeField] private Transform _enemyAfterEvasionMovePoint;
        [Space(10)] 
        [SerializeField] private float _evasionDistance;
        [SerializeField] private float _endCallbackDelay;
        [SerializeField] private SlowMotionEffectSO _slowMotionEffect;
        [SerializeField] private CorrectSwipeChecker _correctSwipeChecker;
        private bool _evaded;
        private bool _playerCompleted;
        private bool _enemyCompleted;
        
        
        public override void Activate()
        {
            Player.Aimer.BeginAim();
            if (_doMoveEnemy)
                Enemy.Mover.MoveToPoint(_enemyPoint, _enemyMoveTime, Punch);                
            else
                Punch();
        }

        public override void Stop()
        {
            _isStopped = true;
            _slowMotionEffect.Stop();
        }

        private void DelayedSwipeOn()
        {
            DelayRealtime(() =>
            {
                _correctSwipeChecker.OnCorrect = OnCorrectSwipe;
                _correctSwipeChecker.OnWrong = OnWrongSwipe;
                _correctSwipeChecker.On();
            }, .25f);
        }
        
        private void Punch()
        {
            if (_isStopped)
                return;
            Enemy.Punch(_animKey, OnPunchStarted, OnPunchEnd, OnEnemyAnimationEnd);
            DelayedSwipeOn();
        }

        private void OnEnemyAnimationEnd()
        {
            CLog.Log($"[{nameof(LevelStagePunch)}] OnAnimationEnd");
            if (_enemyMoveAfterEvaded)
                Enemy.Mover.MoveToPoint(_enemyAfterEvasionMovePoint, _enemyAfterEvasionMoveTime, () => {});
            else
            {
                _enemyCompleted = true;
                if(_waitForBoth && _playerCompleted)
                    Complete();
            }
        }
        
        private void OnCorrectSwipe()
        {
            CLog.Log($"[{nameof(LevelStagePunch)}] On Correct swipe");
            if (_isStopped)
                return;
            _evaded = true;
            SwipeOff();
            PlayerMover.Evade(_correctSwipeChecker.CorrectDirection, OnPlayerEvaded, _evasionDistance);
        }

        private void OnPlayerEvaded()
        {
            _playerCompleted = true;
            if(_resumePlayerMoveRightAfterEvasion)
                PlayerMover.Resume();
            if (_waitForBoth && !_enemyCompleted)
                return;
            Complete();
        }
        
        private void Complete()
        {
            Player.Aimer.BeginAim();
            Delay(() =>
            {
                if (_isStopped)
                    return;
                if(!_resumePlayerMoveRightAfterEvasion)
                    PlayerMover.Resume();
                CallCompleted();
            }, _endCallbackDelay);
        }

        private void OnWrongSwipe()
        {
            CLog.Log($"[{nameof(LevelStagePunch)}] On wrong swipe");
            if (_isStopped)
                return;
            SwipeOff();
            DestroyPlayerAndFail();
        }

        private void SwipeOff()
        {
            UI.EvadeUI.Stop();
            _correctSwipeChecker.Off();
            _slowMotionEffect.Stop();
        }

        private void OnPunchStarted()
        {
            if (_isStopped)
                return;
            Player.Aimer.StopAim();
            Player.Shooter.StopShooting();
            _slowMotionEffect.Begin();
            UI.EvadeUI.AnimateByDirection(_correctSwipeChecker.CorrectDirection);
        }

        private void OnPunchEnd()
        {
            if (_evaded || _isStopped)
                return;
            _slowMotionEffect.Stop();
            DestroyPlayerAndFail();
        }


    }
}