using System.Collections.Generic;
using UnityEngine;

namespace SleepDev.Levels
{
    public class LevelManager : MonoBehaviour, ILevelManager
    {
        // [SerializeField] private LevelsRepository _levelsRepository;
        [SerializeField] private Vector2Int _randomizeLevelLimits;
        [SerializeField] private List<int> _loopExcludedLevels;
        
        private ILevelData _currentLevel;
        public ILevelData CurrentLevel => _currentLevel;
        
        
        public void LoadCurrent()
        {
            var level = GetLevel(GameCore.Core.GCon.PlayerData.LevelTotal);
            _currentLevel = level;
            Load(level.SceneName);
        }

        /// <summary>
        /// Moves level index to next
        /// </summary>
        public void NextLevel()
        {
            GameCore.Core.GCon.PlayerData.LevelTotal++;
            // CLog.Log($"Set level total next: {GC.PlayerData.LevelTotal}");
        }
                
        public void LoadPrev()
        {
            var data = GameCore.Core.GCon.PlayerData;
            data.LevelTotal--;
            if (data.LevelTotal < 0)
                data.LevelTotal = 0;
            var level = GetLevel(GameCore.Core.GCon.PlayerData.LevelTotal);
            _currentLevel = level;
            Load(level.SceneName);   
        }
        
        private ILevelData GetLevel(int index)
        {
            var count = GameCore.Core.GCon.LevelRepository.Count;
            if (index >= count )
            {
                Debug.Log($"[LM] Total levels {index} > levelsCount {count}. Randomizing");
                index = GetRandomIndex(index);
            }
            var level = GameCore.Core.GCon.LevelRepository.GetLevel(index);
            CLog.Log($"[LevelManager] Index {index}, Scene {level.SceneName}, Level {level.LevelName}");
            return level;
        }

        private int GetRandomIndex(int current)
        {
            var index = UnityEngine.Random.Range(_randomizeLevelLimits.x, _randomizeLevelLimits.y);
            const int max_iterations = 50;
            var it_count = 0;
            while ((index == current || _loopExcludedLevels.Contains(index))
                   && it_count < max_iterations)
            {
                index = UnityEngine.Random.Range(_randomizeLevelLimits.x, _randomizeLevelLimits.y);
                it_count++;
            }
            if(it_count >= max_iterations)
                Debug.LogError($"Iterated over {max_iterations} times to get random level index!");
            return index;
        }

        private void Load(string sceneName)
        {
            GameCore.Core.GCon.SceneSwitcher.OpenScene(sceneName, OnLoaded);   
        }
        
        private void OnLoaded(bool success)
        { }

        
        

    }
}