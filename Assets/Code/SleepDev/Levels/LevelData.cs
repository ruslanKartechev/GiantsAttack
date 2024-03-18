using UnityEngine;

namespace SleepDev.Levels
{
    [System.Serializable]
    public class LevelData : ILevelData
    {
        [SerializeField] private string _level;
        [SerializeField] private string _scene;

        public LevelData(){}

        public LevelData(string scene, string level)
        {
            _level = level;
            _scene = scene;
        }

        public string SceneName
        {
            get => _scene;
            set => _scene = value;
        }
        
        public string LevelName => _level;
        
        public GameObject Prefab()
        {
            return Resources.Load<GameObject>($"Prefabs/Levels/{_level}");
        }
    }
}