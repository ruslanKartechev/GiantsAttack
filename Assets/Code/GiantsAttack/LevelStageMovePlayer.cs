using UnityEngine;

namespace GiantsAttack
{
    public class LevelStageMovePlayer : LevelStage
    {
        [SerializeField] private bool _allowShooting;
        [Header("Player Movement")]
        [SerializeField] private Transform _playerMoveToPoint;
        [SerializeField] private float _playerMoveTime;
        [SerializeField] private AnimationCurve _moveCurve;


        public override void Activate()
        {
            if (_allowShooting)
            {
                Player.Aimer.BeginAim();
            }
            else
            {
                Player.Aimer.StopAim();
            }
            Player.Mover.StopLoiter(true);
            Player.Mover.MoveTo(_playerMoveToPoint, _playerMoveTime, _moveCurve, OnMovementDone);
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