using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu(menuName = "SO/" + nameof(LevelsRepository), fileName = nameof(LevelsRepository), order = 0)]
    public class LevelsRepository : ScriptableObject, ILevelRepository
    {
        [SerializeField] private List<LevelData> _levels;
        [SerializeField] private List<EnvData> _envData;
        public List<LevelData> Levels => _levels;
        public int Count => _levels.Count;
        
        public byte GetEnvironmentIndex(string scene)
        {
            return _envData.Find(t => t.sceneName == scene).envIndex;
        }

        public ILevelData GetLevel(int index)
        {
            if (index >= _levels.Count)
                index = 0;
            return _levels[index];   
        }

        [System.Serializable]
        public class EnvData
        {
            public byte envIndex;
            public string sceneName;
        }
        
        #if UNITY_EDITOR

        [ContextMenu("ReverseLevelAndLoc")]
        public void ReverseLevelAndLoc()
        {
            foreach (var data in _levels)
            {
                var scene = data.SceneName;
                var level = data.LevelName;
                data.SceneName = level;
                data.LevelName = scene;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}