using SleepDev.Saving;
using UnityEngine;
using SleepDev;

namespace RaftsWar.Core
{
    
    public class SaveInitializer : MonoBehaviour, ISaveInitializer
    {
        [SerializeField] private bool _applyCheat;
        [SerializeField] private bool _clearLoaded;
        [SerializeField] private IDataSaver _saver;
        [Space(10)] 
        [SerializeField] private PlayerData _playerDataCheat;

        public PlayerData CheatData
        {
            get => _playerDataCheat;
            set => _playerDataCheat = value;
        }
        
        public void InitSavedData()
        {
            if(_clearLoaded)
                _saver.Clear();
            _saver.Load();
            var loaded = _saver.GetLoadedData();
            if (_applyCheat)
            {
                InitPlayerData(_playerDataCheat);
                return;
            }
            InitPlayerData(loaded.PlayerData);
         
            void InitPlayerData(IPlayerData data)
            {
                GCon.PlayerData = new PlayerData(data);
                GCon.TowerRepository.Init(data.TowerLevel, data.TowerProgress);
                GCon.TowerRepository.Init(data.TowerLevel, data.TowerProgress);
            }
        }
        
    }
}