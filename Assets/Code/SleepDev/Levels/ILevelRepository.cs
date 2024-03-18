
namespace SleepDev.Levels
{
    public interface ILevelRepository
    {
        // public EnvironmentLevel GetEnvironment(int index);
        ILevelData GetLevel(int index);
        public int Count { get; }
    }
}