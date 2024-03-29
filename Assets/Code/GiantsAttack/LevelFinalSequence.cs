using System;
using GameCore.Cam;
using SleepDev;

namespace GiantsAttack
{
    public abstract class LevelFinalSequence : MonoExtended
    {
        public IHelicopter Player { get; set; }
        public IMonster Enemy { get; set; }
        public PlayerCamera Camera { get; set; }
        
        public abstract void Begin(Action callback);
    }
}