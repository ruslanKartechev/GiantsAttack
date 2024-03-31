using SleepDev;
using SleepDev.SlowMotion;
using UnityEngine;

namespace GiantsAttack
{
    public class LevelStagePunch : LevelStage
    {
        [SerializeField] private string _animKey;
        [SerializeField] private bool _doMoveEnemy;
        [SerializeField] private float _enemyMoveTime;
        [SerializeField] private Transform _enemyPoint;
        [Space(10)]
        [SerializeField] private bool _doMovePlayer;
        [SerializeField] private AnimationCurve _playerMoveCurve;
        [SerializeField] private float _playerMoveTime;
        [SerializeField] private Transform _playerPoint;
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
            {
                Enemy.Mover.MoveTo(_enemyPoint, _enemyMoveTime, Punch);                
            }
            else
            {
                Punch();
            }
            if (_doMovePlayer)
            {
                Player.Mover.MoveTo(_playerPoint, _playerMoveTime, _playerMoveCurve,() => {});
            }
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
            Enemy.Punch(_animKey, OnPunchStarted, OnPunchEnd);
            DelayedSwipeOn();
        }

        private void OnCorrectSwipe()
        {
            CLog.Log($"On Correct swipe");
            if (_isStopped)
                return;
            _evaded = true;
            Player.Mover.Evade(_correctSwipeChecker.CorrectDirection, OnEvadeMoveEnd, _evasionDistance);
            SwipeOff();
            Player.Aimer.BeginAim();
        }

        private void OnEvadeMoveEnd()
        {
            Delay(CallCompleted, _endCallbackDelay);
        }
        
        private void OnWrongSwipe()
        {
            CLog.Log($"On wrong swipe");
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