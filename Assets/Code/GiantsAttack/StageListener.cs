using SleepDev;

namespace GiantsAttack
{
    public abstract class StageListener : MonoExtended
    {
        public IMonster Enemy { get; set; }
        public abstract void OnActivated();
        public abstract void OnStopped();
        public abstract void OnCompleted();
    }
}