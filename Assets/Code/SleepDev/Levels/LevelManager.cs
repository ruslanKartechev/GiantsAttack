using System.Collections.Generic;
using GameCore.Core;
using UnityEngine;

namespace SleepDev.Levels
{
    public class LevelManager : MonoBehaviour, ILevelManager
    {
        // [SerializeField] private LevelsRepository _levelsRepository;
        [SerializeField] private Vector2Int _randomizeLevelLimits;
        [SerializeField] private List<int> _loopExcludedLevels;
        private int _currentIndex = -1;
        private int _nextIndex = -1;
        private ILevelData _currentLevel;

        public int CurrentIndex => _currentIndex;
        public int NextIndex => _nextIndex;
        public ILevelData CurrentLevel => _currentLevel;

        public void SetupLevels()
        {
            if (_currentIndex == -1)
                _currentIndex = GCon.PlayerData.LevelTotal;
            _currentIndex = CorrectIndex(_currentIndex);
            _nextIndex = GetNextLevel(_currentIndex);
            GCon.PlayerData.LevelTotal = _currentIndex;
        }
        
        /// <summary>
        /// Loads Current Level, also Cashes next level index to be used for next level. Cashes randomzied levels too
        /// </summary>
        public void LoadCurrent()
        {
            SetupLevels();
            _currentLevel = GetLevel(_currentIndex);
            Load(_currentLevel.SceneName);
        }

        /// <summary>
        /// Moves level index to next
        /// </summary>
        public void NextLevel()
        {
            if (_nextIndex == -1)
                _nextIndex = CorrectIndex(++GCon.PlayerData.LevelTotal);
            _currentIndex = _nextIndex;
        }
                
        public void LoadPrev()
        {
            var data = GCon.PlayerData;
            data.LevelTotal--;
            if (data.LevelTotal < 0)
                data.LevelTotal = 0;
            var level = GetLevel(GCon.PlayerData.LevelTotal);
            _currentLevel = level;
            Load(level.SceneName);   
        }

        private int CorrectIndex(int index)
        {
            var count = GameCore.Core.GCon.LevelRepository.Count;
            if (index >= count )
                index = GetRandomIndex(index);
            return index;
        }
        
        private int GetNextLevel(int index)
        {
            index++;
            return CorrectIndex(index);
        }
        
        private ILevelData GetLevel(int index)
        {
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
            GCon.SceneSwitcher.OpenScene(sceneName, OnLoaded);   
        }
        
        private void OnLoaded(bool success)
        { }

        
        

    }
}