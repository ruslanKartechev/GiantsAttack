using System;
using System.IO;
using SleepDev;
using SleepDev.Saving;
using UnityEngine;

namespace RaftsWar.Core
{
    [CreateAssetMenu(menuName = "SO/" + nameof(GameDataSaver), fileName = nameof(GameDataSaver), order = 0)]
    public class GameDataSaver : SleepDev.Saving.IDataSaver
    {
        [NonSerialized] private SavedData _loadedData;

        public override ISavedData GetLoadedData()
        {
            if(_loadedData == null)
                CLog.LogWHeader("DataSaver", "Null saved data!", "r");
            return _loadedData;
        }

        public override void Load()
        {
            if (File.Exists(Path))
            {
                var fileContents = File.ReadAllText(Path);
                _loadedData = JsonUtility.FromJson<SavedData>(fileContents);
                if (_loadedData == null)
                    _loadedData = new SavedData();
            }
            else
            {
                _loadedData = new SavedData();
            }
        }

        public override void Save()
        {
            var playerData = GCon.PlayerData;
            playerData.TowerLevel = GCon.TowerRepository.Level;
            playerData.TowerProgress = GCon.TowerRepository.Progress;
            var gameData = new SavedData(playerData);
            var jsonString = JsonUtility.ToJson(gameData);
            File.WriteAllText(Path, jsonString);
        }
        
        public override void Clear()
        {
            if(Application.isPlaying)
                CLog.LogWHeader("DataSaver", "Saved Data Cleared!", "w");
            File.Delete(Path);
            _loadedData = null;
            #if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
            #endif
        }

        public void LogPath()
        {
            Debug.Log($"Path to saves\n {Path}");
        }
    }
}