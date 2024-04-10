using System;
using SleepDev;

namespace GiantsAttack
{
    public abstract class LevelStartSequence : MonoExtended
    {
        public IMonster Enemy { get; set; }
        
        
        #if UNITY_EDITOR
        public abstract void E_Init();
        #endif
        public abstract void Begin(Action onEnd);
    }
}