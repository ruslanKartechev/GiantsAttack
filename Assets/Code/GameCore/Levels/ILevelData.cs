using UnityEngine;

namespace GameCore
{
    public interface ILevelData
    {
        string SceneName { get; }
        string LevelName { get; }
        GameObject Prefab();
    }
}