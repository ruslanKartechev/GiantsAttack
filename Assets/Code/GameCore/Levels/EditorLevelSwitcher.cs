using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
namespace GameCore
{
    #if UNITY_EDITOR
    public class EditorLevelSwitcher
    {
        private LevelsRepository _levelRepository;
        private static int _currentIndex;
        private GameObject _currentInstance;

        public static bool LoadOnSelect { get; set; } = true;
        public static bool ClearOnLoad { get; set; } = true;
        public static string LevelsPath { get; set; } = "Prefabs/Levels/";
        
        
        public static int CurrentIndex
        {
            get => _currentIndex;
            set => _currentIndex = value;
        }
        
        public int Index
        {
            get => _currentIndex;
            set => _currentIndex = value;
        }

        public LevelsRepository LevelRepository
        {
            get => _levelRepository;
            set => _levelRepository = value;
        }

        public EditorLevelSwitcher(LevelsRepository levelRepository)
        {
            _levelRepository = levelRepository;
            CorrectIndex();
        }
        
        public void NextLevel()
        {
            _currentIndex++;
            CorrectIndex();
            if(IsValidIndex() && LoadOnSelect)
                LoadCurrentLevel();
        }

        public void PrevLevel()
        {
            _currentIndex--;
            CorrectIndex();
            if(IsValidIndex() && LoadOnSelect)
                LoadCurrentLevel();
        }

        public void LoadCurrentLevel()
        {
            if (ClearOnLoad)
                Clear();
            var level = _levelRepository.GetLevel(_currentIndex);
            try
            {
                EditorSceneManager.OpenScene($"Assets/Scenes/{level.SceneName}.unity");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{ex.Message}");
            }
            var prefab = Resources.Load(LevelsPath + $"{level.LevelName}");
            if (prefab == null)
            {
                Debug.LogError($"Prefab {level.LevelName} not found");
                return;
            }
            var inst = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            _currentInstance = inst;
        }

        public void Clear()
        {
            if (_currentInstance != null)
            {
                UnityEngine.Object.DestroyImmediate(_currentInstance);
            }
        }
        
        public void CorrectIndex()
        {
            if (_levelRepository.Count == 0)
                throw new System.Exception("No levels assigned to level repository list");
            if (_currentIndex < 0)
                _currentIndex = 0;
            if (_currentIndex >= _levelRepository.Count)
                _currentIndex = _levelRepository.Count - 1;
        }

        public bool IsValidIndex()
        {
            if (_levelRepository.Count == 0)
                return false;
            if (_currentIndex < 0)
                return false;
            if (_currentIndex >= _levelRepository.Count)
                return false;
            return true;
        }

        public string GetLevelStatus(ILevelData level)
        {
            var results = UnityEditor.AssetDatabase.FindAssets(level.LevelName, new[] { "Assets/Resources" });
            if (results.Length > 0)
            {
                return "OK";
            }
            return "not found";
        }

        public string GetSceneStatus(ILevelData level)
        {
            var results = UnityEditor.AssetDatabase.FindAssets(level.SceneName, new[] { "Assets/Scenes" });
            if (results.Length > 0)
            {
                var count = SceneManager.sceneCountInBuildSettings;
                for (var i = 0; i < count; i++)
                {
                    var scene = SceneManager.GetSceneByBuildIndex(i);
                    if (scene.name == level.SceneName)
                        return "OK";
                }

                return "not in build";
            }
            return "not found";
        }

        public void CheckAll()
        {
            
        }
        
        public void AddFromResources()
        {
            
        }

        public void TryOrder()
        {
            
        }
        
        public void Randomize()
        {
            
        }
    }
    #endif
}