using GameCore.Cam;
using GameCore.UI;

namespace GiantsAttack
{
    [System.Serializable]
    public class HelicopterInitArgs
    {
        public AimerSettings aimerSettings;
        public ShooterSettings shooterSettings;
        public MoverSettings moverSettings;
        
        public IHitCounter hitCounter;
        public IPlayerCamera camera;
        public IControlsUI controlsUI;
        public IAimUI aimUI;

    }
}