using System;
using GameCore.Cam;
using GameCore.Core;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public class FailSequenceRoar : LevelFailSequence
    {
        [SerializeField] private float _delay = 1;
        [SerializeField] private float _camMoveTime = 1;
        private Action _callback;
        
        
        public override void Play(Action onEnd)
        {
            _callback = onEnd;
            Player.Mover.StopAll();
            Player.StopAll();
            Enemy.Mover.StopMovement();
            Delay(ShowScreen,_delay);
        }

        private void ShowScreen()
        {
            Enemy.Roar();
            CameraContainer.PlayerCamera.Parent(null);
            CameraContainer.PlayerCamera.MoveToPoint(Enemy.CameraFacePoint, _camMoveTime, _callback);
            var ui = (IGameplayMenu)GCon.UIFactory.GetGameplayMenu();
            ui.Hide(() => {});
        }
    }
}