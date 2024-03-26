﻿
namespace GiantsAttack
{
    public interface IHelicopter
    {
        IHelicopterMover Mover { get; }
        IHelicopterShooter Shooter { get; }
        IHelicopterAimer Aimer { get; }
        IDamageable Damageable { get; }
        IHelicopterCameraPoints CameraPoints { get; }
        IDestroyer Destroyer { get; }
        void Init(HelicopterInitArgs args);
        
    }
    
}