using GameCore.UI;

namespace GiantsAttack
{
    public interface IHelicopterAimer
    {
        public AimerSettings Settings { get; set; }
        void Init(AimerSettings settings, IHelicopterShooter shooter, IControlsUI controlsUI, IAimUI aimUI);
        void BeginAim();
        void StopAim();
        void Reset();
        void SetInitialRotation();
        public IAimUI AimUI { get; set; }

    }
}