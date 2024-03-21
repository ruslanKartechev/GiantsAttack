using SleepDev.Saving;
using UnityEngine;

namespace GameCore.Core
{
    [System.Serializable]
    public class SavedData : ISavedData
    {
        [SerializeField] private PlayerData _playerData;
        public IPlayerData PlayerData => _playerData;
        

        public SavedData()
        {
            _playerData = new PlayerData();
        }

        public SavedData(IPlayerData playerData)
        {
            _playerData = new PlayerData(playerData);
        }
 
    }
}