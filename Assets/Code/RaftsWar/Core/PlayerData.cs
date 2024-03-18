using UnityEngine;

namespace RaftsWar.Core
{
    [System.Serializable]
    public class PlayerData : IPlayerData
    {
        [SerializeField] private float _money;
        [SerializeField] private int _levelsTotal;
        [SerializeField] private int _towerLevel;
        [SerializeField] private float _towerProgress;
        
        public PlayerData(){}
        
        public PlayerData(IPlayerData from)
        {
            _money = from.Money;
            _levelsTotal = from.LevelTotal;
            _towerLevel = from.TowerLevel;
            _towerProgress = from.TowerProgress;
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

        public int TowerLevel
        {
            get => _towerLevel;
            set => _towerLevel = value;
        }
        
        public float TowerProgress
        {
            get => _towerProgress;
            set => _towerProgress = value;
        }
    }
}