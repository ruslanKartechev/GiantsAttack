using UnityEngine;

namespace GameCore.Core
{
    [System.Serializable]
    public class PlayerData : IPlayerData
    {
        [SerializeField] private float _money;
        [SerializeField] private int _levelsTotal;
        [SerializeField] private bool _soundStatus = true;
        [SerializeField] private float _soundVolume = 1f;
        [SerializeField] private bool _vibrationStatus = true;

        
        public PlayerData(){}
        
        public PlayerData(IPlayerData from)
        {
            _money = from.Money;
            _levelsTotal = from.LevelTotal;
            _soundStatus = from.SoundStatus;
            _soundVolume = from.SoundVolume;
            _vibrationStatus = from.VibrationStatus;
        }
        
        public float Money
        {
            get => _money;
            set => _money = value;
        }
        

        public int LevelTotal
        {
            get => _levelsTotal;
            set => _levelsTotal = value;
        }
        
        public bool SoundStatus
        {
            get => _soundStatus;
            set => _soundStatus = value;
        }
        public float SoundVolume
        {
            get => _soundVolume;
            set => _soundVolume = value;
        }
        public bool VibrationStatus
        {
            get => _vibrationStatus;
            set => _vibrationStatus = value;
        } 
        
    }
}