using SleepDev;
using UnityEngine;

namespace RaftsWar.Levels
{
    public abstract class Level : MonoExtended
    {
        public abstract void InitLevel();
        public abstract Transform StartCameraPoint { get; }
    }
}