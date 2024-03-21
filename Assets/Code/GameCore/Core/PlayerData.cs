using UnityEngine;

namespace GameCore.Core
{
    [System.Serializable]
    public class PlayerData : IPlayerData
    {
        [SerializeField] private float _money;
        [SerializeField] private int _levelsTotal;

        
        public PlayerData(){}
        
        public PlayerData(IPlayerData from)
        {
            _money = from.Money;
            _levelsTotal = from.LevelTotal;
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
        
    }
}