using SleepDev;
using SleepDev.SlowMotion;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStagePunch : LevelStage
    {
        [SerializeField] private string _animKey;
        [SerializeField] private bool _resetAnimRootBone;
        [SerializeField] private bool _idleAfterReset = true;
        [SerializeField] private bool _doMoveEnemy;
        [SerializeField] private float _enemyMoveTime;
        [SerializeField] private Transform _enemyPoint;
        [Space(10)]
        [SerializeField] private bool _doMovePlayer;
        [SerializeField] private bool _loiterPlayer;
        [SerializeField] private AnimationCurve _playerMoveCurve;
        [SerializeField] private float _playerMoveTime;
        [SerializeField] private Transform _playerPoint;

        [Space(10)] [Header("After evasion movement Player")] 
        [SerializeField] private EWaitForMovement _waitType;
        [SerializeField] private bool _moveAfterEvaded;
        [SerializeField] private float _afterEvasionMoveTime;
        [SerializeField] private Transform _afterEvasionMovePoint;
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
        
        public override void Activate()
        {
            Player.Aimer.BeginAim();
            if (_doMoveEnemy)
                Enemy.Mover.MoveTo(_enemyPoint, _enemyMoveTime, Punch);                
            else
                Punch();
            if (_doMovePlayer)
                Player.Mover.MoveTo(_playerPoint, _playerMoveTime, _playerMoveCurve,() => {});
            else if (_loiterPlayer)
                Player.Mover.Loiter();
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
            Enemy.Punch(_animKey, OnPunchStarted, OnPunchEnd, OnAnimationEnd);
            DelayedSwipeOn();
        }

        private void OnAnimationEnd()
        {
            CLog.Log($"[{nameof(LevelStagePunch)}] OnAnimationEnd");
            if (_enemyMoveAfterEvaded)
            {
                Enemy.Mover.MoveTo(_enemyAfterEvasionMovePoint, _enemyAfterEvasionMoveTime, () => {});
                return;
            }
            if(_resetAnimRootBone)
                Enemy.AlignPositionToAnimRootBone(_idleAfterReset);
        }

        private void OnEnemyMoved()
        {
            
        }

        private void OnCorrectSwipe()
        {
            CLog.Log($"[{nameof(LevelStagePunch)}] On Correct swipe");
            if (_isStopped)
                return;
            _evaded = true;
            SwipeOff();
            Player.Mover.Evade(_correctSwipeChecker.CorrectDirection, OnEvadeMoveEnd, _evasionDistance);
            Player.Aimer.BeginAim();
        }

        private void OnEvadeMoveEnd()
        {
            if (_moveAfterEvaded)
            {
                Player.Mover.MoveTo(_afterEvasionMovePoint, _afterEvasionMoveTime, null, Complete);        
            }
            else
            {
                Complete();
            }
        }
        

        private void Complete()
        {
            Player.Mover.Loiter();
            Delay(CallCompleted, _endCallbackDelay);
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