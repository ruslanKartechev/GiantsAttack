using UnityEngine;

namespace GiantsAttack
{
    public interface IHelicopterShooter
    {
        ShooterSettings Settings { get; set; }
        Transform FromPoint { get; set; }
        Transform AtPoint { get; set; }
        IHitCounter HitCounter { get; set; }
        
        void Init(ShooterSettings settings, IHitCounter hitCounter);
        void StopShooting();
        void BeginShooting();
        IHelicopterGun Gun { get; set; }
        void RotateToScreenPos(Vector3 aimPos);
    }
}