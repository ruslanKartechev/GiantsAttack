using System;
using GameCore.Cam;
using UnityEngine;

namespace GiantsAttack
{
    public interface IHelicopterCameraPoints
    {
        Transform InsidePoint { get; }
        Transform OutsidePoint { get; }
        void SetCamera(IPlayerCamera camera);
        public void MoveCameraToInside(Action callback);
        public void MoveCameraToOutside(Action callback);
        
        public void SetCameraToOutside();
        public void SetCameraInside();
    }
}