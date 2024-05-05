using System;
using UnityEngine;

namespace GiantsAttack
{
    public class GodzillaFinalSequence : LevelFinalSequence
    {
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
            Delay(() =>
            {
                Player.Mover.CenterInternal();
                Player.Mover.ParentAndMoveLocal(Enemy.KillPoint, _playerRotateTime, 
                    AnimationCurve.EaseInOut(0f, .5f, 1f, 1f), null);
                BuildingHider.HideBuildingsUnder(Enemy.KillPoint.position, Enemy.KillPoint.forward);
            }, _playerMoveDelay);
            callback.Invoke();
        }
    }
}