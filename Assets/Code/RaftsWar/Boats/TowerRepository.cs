using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CreateAssetMenu(menuName = "SO/TowerRepository", fileName = "TowerRepository", order = 0)]
    public class TowerRepository : ScriptableObject, ITowerRepository
    {
        [SerializeField] private string _resourcesPath;
        [SerializeField] private List<string> _towerNames;
        [SerializeField] private List<SpriteFillIcon> _sprites;
        
        private int Count => _towerNames.Count;
        public int Level { get; private set; }
        public float Progress { get; set;}
        
        
        public void Init(int currentLevel, float progress)
        {
            Level = currentLevel;
            Progress = progress;
            // CLog.Log($"[TowerRepository] Level set {currentLevel}, progress set {progress}");
        }

        public void Upgrade()
        {
            if (Level >= Count - 1)
            {
                Progress = 1f;
                return;
            }
            Level++;
            Progress = 0f;
        }

        public bool CanUpgrade()
        {
            return Level < Count - 1;
        }
        
        public SpriteFillIcon NextTowerSprite()
        {
            if (Level + 1 >= Count)
                return _sprites[^1];
            return _sprites[Level+1];
        }

        public GameObject GetCurrentPrefab()
        {
            var path = (_resourcesPath + _towerNames[Level]);
            var go = Resources.Load<GameObject>(path);
            return go;
        }

        public ITower GetCurrentPrefabTower()
        {
            var path = string.Join(_resourcesPath, _resourcesPath[Level]);
            var go = Resources.Load<GameObject>(path);
            return go.GetComponent<ITower>();
        }
    }
}