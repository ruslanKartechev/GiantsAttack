using System;
using GameCore.Cam;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class HelicopterInitArgs
    {
        public ShooterSettings shooterSettings;
        public Transform enemyTransform;
        
        [NonSerialized] public AimerSettings aimerSettings;
        public IHitCounter hitCounter;
        public IPlayerCamera camera;
        public IControlsUI controlsUI;
        public IAimUI aimUI;
    }
}