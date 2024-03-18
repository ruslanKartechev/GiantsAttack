using UnityEngine;

namespace SleepDev.Levels
{
    public interface ILevelData
    {
        string SceneName { get; }
        string LevelName { get; }
        GameObject Prefab();
    }
}