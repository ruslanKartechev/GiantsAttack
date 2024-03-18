using System.Collections.Generic;
using UnityEngine;

namespace SleepDev.Levels
{
    [CreateAssetMenu(menuName = "SO/" + nameof(LevelsRepository), fileName = nameof(LevelsRepository), order = 0)]
    public class LevelsRepository : ScriptableObject, ILevelRepository
    {
        [SerializeField] private List<LevelData> _levels;
        
        public int Count => _levels.Count;

        public ILevelData GetLevel(int index)
        {
            if (index >= _levels.Count)
                index = 0;
            return _levels[index];   
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Add levels")]
        public void AddLevels()
        {
            var start = _levels.Count;
            var end = 50;
            var scene = _levels[0].SceneName;
            // for (var i = start; i <= end; i++)
            // {
            //     var levelName = $"Level_{i}";
            //     _levels.Add(new LevelData(scene, levelName));
            // }
            for(var i = 29; i < 40; i++)
            {
                _levels[i].SceneName = "Levels_2";
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
}