using GameCore.Cam;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class HelicopterInitArgs
    {
        public AimerSettings aimerSettings;
        public ShooterSettings shooterSettings;
        public MoverSettings moverSettings;
        public Transform enemyTransform;
        
        public IHitCounter hitCounter;
        public IPlayerCamera camera;
        public IControlsUI controlsUI;
        public IAimUI aimUI;

    }
}