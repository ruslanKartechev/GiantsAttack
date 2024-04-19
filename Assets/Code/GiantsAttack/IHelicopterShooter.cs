using UnityEngine;
using GameCore.UI;

namespace GiantsAttack
{
    public interface IHelicopterShooter
    {
        ShooterSettings Settings { get; set; }
        Transform FromPoint { get; set; }
        Transform AtPoint { get; set; }
        IHitCounter HitCounter { get; set; }
        IHelicopterGun Gun { get; set; }
        void Init(ShooterSettings settings, IHitCounter hitCounter);
        void StopShooting();
        void BeginShooting();
        void RotateToScreenPos(Vector3 aimPos);
    }
}