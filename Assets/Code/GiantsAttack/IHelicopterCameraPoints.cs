using RaftsWar.Cam;
using UnityEngine;

namespace GiantsAttack
{
    public interface IHelicopterCameraPoints
    {
        Transform InsidePoint { get; }
        Transform OutsidePoint { get; }
        void SetCamera(IPlayerCamera camera);

        public void MoveCameraToInside();
        public void SetCameraToOutside();
        public void SetCameraInside();
    }
}