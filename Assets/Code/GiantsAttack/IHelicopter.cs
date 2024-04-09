
using System.Collections.Generic;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public interface IHelicopter
    {
        IHelicopterMover Mover { get; }
        IHelicopterShooter Shooter { get; }
        IHelicopterAimer Aimer { get; }
        IDamageable Damageable { get; }
        IHelicopterCameraPoints CameraPoints { get; }
        IBodySectionsUI BodySectionsUI { get; }
        IDestroyer Destroyer { get; }
        void Init(HelicopterInitArgs args);
        void StopAll();
        void Kill();
        Transform Point { get; }
    }
    
}