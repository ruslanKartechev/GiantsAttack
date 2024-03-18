using System.Collections.Generic;
using SleepDev;
using UnityEngine;
using UnityEngine.Assertions;

namespace RaftsWar.Boats
{
    public class TowerViewSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private TowerView _spawnedView;
        [SerializeField] private PrimaryTowerView _primaryTower;
        public TowerView View => _spawnedView;
        public TowerSettings Settings { get; set; }
        public int Level { get; set; } = 0;
        
        public List<Transform> UnitsSpawnPoints { get; set; }

        public void Init(TowerSettings settings)
        {
            CLog.Log($"[Tower] Init {gameObject.name}");
            Settings = settings;
            UnitsSpawnPoints = new List<Transform>();
            SetSpawnPoints();
        }

        private void SetSpawnPoints()
        {
            foreach (var point in _spawnedView.UnitSpawnPoint)
                UnitsSpawnPoints.Add(point);
        }
        
        public void UpdateToLevel(int level)
        {
            if (level == 0)
            {
                CLog.Log($"[ViewSpawner] level 0, main tower already spawned");
                return;
            }
            Level = level;
            if (level == 1)
            {
                SpawnNewPrimaryTower(level);
                return;
            }
            // CLog.Log($"[ViewSpawner] Spawning accessory level: {level}");
            var tower = _primaryTower.SpawnAccessory(level - 2);
            foreach (var pp in tower.UnitSpawnPoint)
                UnitsSpawnPoints.Add(pp);
        }

        private void SpawnNewPrimaryTower(int level)
        {
        }

    }
}