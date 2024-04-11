using System;
using SleepDev;

namespace GiantsAttack
{
    public abstract class LevelFailSequence : MonoExtended
    {
        public IMonster Enemy { get; set; }
        public IHelicopter Player { get; set; }

        public abstract void Play(Action onEnd);
    }
}