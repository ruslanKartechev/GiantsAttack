
namespace GameCore
{
    public interface ILevelRepository
    {
        // public EnvironmentLevel GetEnvironment(int index);
        ILevelData GetLevel(int index);
        public int Count { get; }
        byte GetEnvironmentIndex(string scene);
    }
}