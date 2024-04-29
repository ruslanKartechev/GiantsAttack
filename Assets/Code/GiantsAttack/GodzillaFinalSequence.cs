using System;
using UnityEngine;

namespace GiantsAttack
{
    public class GodzillaFinalSequence : LevelFinalSequence
    {
        [SerializeField] private float _enemyRotTime = .33f;
        [SerializeField] private float _playerRotateTime = 1f;
        [SerializeField] private float _playerMoveDelay = .5f;

#if UNITY_EDITOR
        public override void E_Init()
        { }
#endif
        public override void Begin(Action callback)
        {
            Player.StopAll();
            PlayerMover.Pause(false);
            Enemy.PreKillState();
            // Enemy.Mover.RotateToLookAt(Player.Point, _enemyRotTime, () => {});
            // Player.Mover.RotateToLook(Enemy.Point.position + Vector3.up * _lookAtEnemyUpOffset, _playerRotateTime, () => {});
            Delay(() =>
            {
                Player.Mover.MoveTo(new HelicopterMoveToData(Enemy.KillPoint, _playerRotateTime, 
                    AnimationCurve.EaseInOut(0f, .5f, 1f, 1f), 
                    null, () =>{ }));
            }, _playerMoveDelay);
            callback.Invoke();
        }
    }
}