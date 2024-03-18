using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class PrimaryTowerView : MonoBehaviour
    {
        [SerializeField] private List<Transform> _accessorySpawnPoints;
        [SerializeField] private TowerView _mainTower;
        [SerializeField] private GameObject _prefab;
        private List<TowerView> _spawned = new List<TowerView>();

        private void Awake()
        {
            _spawned.Add(_mainTower);
        }

        public Transform GetAccessorySpawn(int level)
        {
            if (level >= _accessorySpawnPoints.Count)
            {
                Debug.LogError("[PrimaryTower] out of range index");
                return null;
            }
            return _accessorySpawnPoints[level];
        }

        public TowerView SpawnAccessory(int level)
        {
            var spawnpoint = GetAccessorySpawn(level);
            if (spawnpoint == null)
                return null;
            // var instance = Instantiate(_prefab,transform);
            // instance.transform.CopyPosRot(spawnpoint);
            // var view = instance.GetComponent<TowerView>();
            var view = spawnpoint.GetComponent<TowerView>();
            view.gameObject.SetActive(true);
            _spawned.Add(view);
            return view;
        }
        
    }
}