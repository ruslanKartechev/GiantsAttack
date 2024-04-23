using System;
using UnityEngine;

namespace GiantsAttack
{
    public class GodzillaFinalSequence : LevelFinalSequence
    {
        [SerializeField] private float _enemyRotTime = .33f;
        [SerializeField] private float _playerRotateTime = 1f;
        [SerializeField] private float _lookAtEnemyUpOffset = 20;

#if UNITY_EDITOR
        public override void E_Init()
        { }
#endif
        public override void Begin(Action callback)
        {
            Player.StopAll();
            PlayerMover.Pause(true);
            Player.Mover.RotateToLook(Enemy.Point.position + Vector3.up * _lookAtEnemyUpOffset, _playerRotateTime, () => {});
            Enemy.PreKillState();
            Enemy.Mover.RotateToLookAt(Player.Point, _enemyRotTime, () => {});
            callback.Invoke();
        }
    }
}