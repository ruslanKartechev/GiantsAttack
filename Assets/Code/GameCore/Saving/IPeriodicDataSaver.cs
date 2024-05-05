namespace SleepDev.Saving
{
    public interface IPeriodicDataSaver
    {
        void SetInterval(float interval);
        void Begin();
    }
}