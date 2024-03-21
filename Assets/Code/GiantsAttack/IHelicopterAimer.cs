using GameCore.UI;

namespace GiantsAttack
{
    public interface IHelicopterAimer
    {
        public AimerSettings Settings { get; set; }
        void Init(AimerSettings settings, IHelicopterShooter shooter, IControlsUI controlsUI);
        void BeginAim();
        void StopAim();
        void Reset();
    }
}