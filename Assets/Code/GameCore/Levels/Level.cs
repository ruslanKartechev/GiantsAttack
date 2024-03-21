using SleepDev;

namespace GameCore.Levels
{
    public abstract class Level : MonoExtended
    {
        public abstract void Init();
        public abstract void Win();
        public abstract void Fail();
        public abstract void Pause();
        public abstract void Resume();
    }
}