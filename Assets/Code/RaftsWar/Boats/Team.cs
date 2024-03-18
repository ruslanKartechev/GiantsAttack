using UnityEngine;
using UnityEngine.Assertions;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class Team
    {
        [SerializeField] protected string _boatName;
        [SerializeField] protected TowerSpawnPoint _spawnPoint;
        [SerializeField] protected TowerSettingsSo _towerSettings;
        [SerializeField] protected BoatSettingsSo _boatSettings;
        [SerializeField] protected BoatViewSettingsSo _boatView;
        [SerializeField] protected UnitViewSettingsSo _unitsView;
        [SerializeField] protected CatapultSettingsSo _catapultSettings;
        [SerializeField] protected CatapultViewSo _catapultViewSo;
        public ITower Tower { get; set; }
        public ITeamPlayer Player { get; set; }
        public TowerSpawnPoint SpawnPoint => _spawnPoint;
        public BoatSettings BoatSettings => _boatSettings.settings;
        public BoatViewSettings BoatView => _boatView.settings;
        public TowerSettings TowerSettings => _towerSettings.settings;
        public UnitViewSettings UnitsView => _unitsView.settings;
        public CatapultSettings CatapultSettings => _catapultSettings.settings;
        public CatapultViewSettings CatapultViewSettings => _catapultViewSo.settings;
        public string BoatName => _boatName;
        public IBoatPartReceiver CurrentPartReceiver { get; set; }
        
        public virtual void InitTower(int startLevel = 0)
        {
#if UNITY_EDITOR
            Assert.IsNotNull(Tower);
#endif
            Tower.Init(this, startLevel);
        }

        public virtual void Stop()
        {
            Tower.Stop();
        }

    }
}