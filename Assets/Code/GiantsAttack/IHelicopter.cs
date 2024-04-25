using UnityEngine;

namespace GiantsAttack
{
    public interface IHelicopter
    {
        IHelicopterMover Mover { get; }
        IHelicopterShooter Shooter { get; }
        IHelicopterAimer Aimer { get; }
        IHelicopterCameraPoints CameraPoints { get; }
        void Init(HelicopterInitArgs args);
        void StopAll();
        void Kill();
        Transform Point { get; }
    }
    
}