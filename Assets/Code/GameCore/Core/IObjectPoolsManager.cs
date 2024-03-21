namespace GameCore.Core
{
    public interface IObjectPoolsManager
    {
        void BuildPools();
        void RecollectAll();
    }
}