using System;
using RaftsWar.UI;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class Tower : MonoExtended, ITower
    {
        public event Action<ITower> OnDestroyed; 

        [SerializeField] protected TowerCollider _towerCollider;
        [SerializeField] protected TowerViewByLevels _viewSpawner;
        [SerializeField] protected TowerRadius _radius;
        [SerializeField] protected BoatPartsProgBar _progBar;
        [SerializeField] protected TowerDamageable _damageable;
        [SerializeField] protected CatapultSpawner _catapultSpawner;
        [SerializeField] protected DamagePointsProvider _damagePoints;
        [SerializeField] protected Transform _cameraPoint;
        [SerializeField] protected TowerFlag _flag;
        [SerializeField] protected TowerBuildingAnimator _buildingAnimator;
        [SerializeField] protected RaftAcceptArea _acceptArea;
        protected ITowerBlocksBuilder _builder;
        protected ITowerUnitsController _unitsController;
        protected TowerSettings _settings;
        protected UnloadPointsManager _unloadPointsManager;
        protected TowerGridOutPusher _gridOutPusher;
        protected ArrowStuckTarget _arrowStuck;
        protected bool _canUpgrade = true;
        protected bool _isDestroyed;
        
        public bool IsDestroyed => _isDestroyed;
        public bool IsMaxLevel => Level == 4;
        public ICatapult Catapult => _catapultSpawner.CatapultInstance;
        public Transform DestroyedCameraPoint => _cameraPoint;
        public Transform Point => transform;
        public IUnloadPointsManager UnloadPointsManager => _unloadPointsManager;
        public IDamageable Damageable => _damageable;
        public IArrowStuckTarget ArrowStuckTarget => _arrowStuck;
        public IDamagePointsProvider DamagePointsProvider => _damagePoints;
        public bool UseUIDamageEffect { get; set; }
        public IUIDamagedEffect UIDamagedEffect { get; set; }

        public Team Team
        {
            get => _damageable.Team;
            set => _damageable.Team = value;
        }

        private Transform CatapultSpawnPoint
        {
            get => _catapultSpawner.SpawnPoint;
            set => _catapultSpawner.SpawnPoint = value;
        }

        public int Level { get; protected set; } = 0;
        public TowerLevelSettings CurrentSettings => _settings.levelSettings[Level];

        #region EditorOnly
#if UNITY_EDITOR
        public int debugLevel;

        public void E_NextLevel()
        {
            debugLevel++;
            debugLevel = Mathf.Clamp(debugLevel, 0, 4);
            E_SetLevel();
        }
        
        public void E_PrevLevel()
        {
            debugLevel--;
            debugLevel = Mathf.Clamp(debugLevel, 0, 4);
            E_SetLevel();
        }

        public void E_ZeroLevel()
        {
            debugLevel = 0;
            E_SetLevel();
        }
       
        public void E_SetLevel()
        {
            var level = debugLevel;
            GetBuilder();
            if (level > 0)
            {
                _radius.Show();
                _flag.Hide();   
            }
            else
            {
                _radius.Hide();
                _flag.Show();
            }
            _viewSpawner.E_Spawn(level);
        }
       
        
        private void E_TowerInit(Team team, int level)
        {
            CatapultSpawnPoint = team.SpawnPoint.catapultSpawnPoint;
            var goFactory = GameObjectFactory.GetDefault();
            GCon.GOFactory = goFactory;
            Level = level;
            GetBuilder();
            var levelSettings = team.TowerSettings.levelSettings[level];
            _radius.SetView(team.TowerSettings.radiusMaterial);
            _radius.SetRadius(levelSettings.radius);
            if (Level > 0)
            {
                _radius.Show();
                _flag.Hide();   
            }
            else
            {
                _radius.Hide();
                _flag.Show();
                _flag.SetView(team);
            }
            _viewSpawner.E_Spawn(level);
            _builder.BuildUpToLevel(team, level);
            if (_catapultSpawner.CheckCondition(level))
            {
                _catapultSpawner.E_Spawn(team);
            }
            _buildingAnimator.ShowAt(_builder);
        }

#endif
        
        public void E_HideViewLevel(int level)
        {
            _viewSpawner.Views[level].HideAnimated();
        }

        public void E_HideBuildingArea(int level)
        {
            _builder.HideAtLevel(level);
        }
        #endregion

        
        public void Init(Team team, int startLevel = 0)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                E_TowerInit(team, startLevel);
                return;
            }
#endif
            Level = startLevel;
            Initiate(team);
            var levelSettings = CurrentSettings;
            InitBuilder();
            InitRadius(levelSettings);
            InitHealth(levelSettings);
            SetupUnloadPoints(_builder);
            // _towerCollider.SetupForAllLevels();
            _viewSpawner.SetLevel(Level);
            _unitsController.UpdateSettings(levelSettings);
            _unitsController.SpawnUnitsAtPoints(_viewSpawner.ActiveSpawnPoints);
            TeamsTargetsManager.Inst.AddTower(this);
            _damageable.CanDamage = Level >= 1;
            _damageable.OnDamaged += OnDamaged;
            if (Level == 0)
            {
                _flag.SetView(team);
                _flag.Show();
            }
            else
                _flag.Hide();
            _buildingAnimator.ShowAt(_builder);
            _acceptArea.SetSquareToLevel(Level);
            Team.CurrentPartReceiver =this;
            foreach (var view in _viewSpawner.Views)
                _damagePoints.AddPoints(view.DamagePoints);
        }

        private void OnDamaged()
        {
            if(UseUIDamageEffect)
                UIDamagedEffect.PlayLong();
        }

        private void Initiate(Team team)
        {
            Team = team;
            _settings = team.TowerSettings;
            CatapultSpawnPoint = team.SpawnPoint.catapultSpawnPoint;
            _viewSpawner.OnUpdatedCallback = OnViewUpdated;
            _arrowStuck = new ArrowStuckTarget(transform);
            _viewSpawner.Init(team, _damageable, _arrowStuck, _damagePoints);
            GetBuilder();
            InitUnits();
            InitCollider();
            InitLevelDisplay();
        }
        
        private void InitUnits()
        {
            GetUnitsController();
            _unitsController.Tower = this;
            _unitsController.UpdateSettings(_settings.levelSettings[Level]);
            if (Level > 0)
                _unitsController.SpawnUnitsAtPoints(_viewSpawner.ActiveSpawnPoints);
            _unitsController.SetOptionalPoints(_viewSpawner.OptionalSpawnPoints);
        }
        
        public bool CanTake() => _builder.CanAccept && !_isDestroyed;
        
        public Square2D GetAreaSquare()
        {
            return _acceptArea.CurrentSquare;
        }

        public void Activate()
        {
            // CLog.LogRed($"[TOWER {gameObject.name}] ACTIVATED, ACTIVATING UNITS");
            _isDestroyed = false;
            if(Level >= 1)
                _unitsController.ActivateAttack();
        }

        public void Stop()
        {
            if (_isDestroyed)
                return;
            _unitsController.StopAttack();
        }

        [ContextMenu("Destroy Tower")]
        public void Kill()
        {
            CLog.LogWhite($"[Tower] {gameObject.name} Destroyed]");
            _isDestroyed = true;
            StopAllCoroutines();
            _towerCollider.Off();
            _progBar.Off();
            _unitsController.KillUnits();
            _viewSpawner.BreakTower();
            _builder.Destroy();
            _radius.ScaleToHide();
            _arrowStuck.HideAll();
            _flag.Hide();
            _buildingAnimator.Hide();
            _damageable.HideDisplay();
            _damageable.OnDamaged -= OnDamaged;
            Destroy((MonoBehaviour)_builder);
            RaiseDestroyed();
        }

        private void RaiseDestroyed()
        {
            OnDestroyed?.Invoke(this);
        }

        public void TakeBoatPart(BoatPart part)
        {
            if (!_canUpgrade) return;
            part.SetView(Team.BoatView);
            _builder.TakeBuildingBlock(part.GetComponent<IBuildingBlock>());
            if (part.HasUnit)
                _unitsController.TakeUnit(part.Unit);
            AddProgressS();
        }
        
        private void GetBuilder()
        {
            _builder = GetComponent<ITowerBlocksBuilder>();
            if (_builder == null)
                CLog.LogRed($"[Tower] ITowerBlocksBuilder is null");
        }
        
        private void GetUnitsController()
        {
            _unitsController = GetComponent<ITowerUnitsController>();
#if UNITY_EDITOR
            UnityEngine.Assertions.Assert.IsNotNull(_unitsController);
#endif
        }
        
        private void InitRadius(TowerLevelSettings levelSettings)
        {
            _radius.SetView(_settings.radiusMaterial);
            _radius.SetRadius(levelSettings.radius);
            if(Level == 0)
                _radius.Hide();
            else
                _radius.Show();
        }

        private void InitLevelDisplay()
        {
            if (Level == 0)
            {
                _progBar.On();
                _progBar.SetCount(_builder.Stored, _builder.Area);
            }
            else
                _progBar.Off();
        }

        private void InitBuilder()
        {
            _builder.BuildUpToLevel(Team, Level);
            _builder.OnCanUpgrade += OnBuilderUpgrade;
        }

        private void InitHealth(TowerLevelSettings levelSettings)
        {
            _damageable.Init(levelSettings.towerHealth);
            _damageable.OnDied += DieFromDamage;
        }
        
        
        private void InitCollider()
        {
            _towerCollider.Init();
        }
        
        private void OnViewUpdated()
        {
            if (_isDestroyed)
                return;
            SpawnUnits();
            if(Level == 1)
                _unitsController.UnloadFromStash();
        }

        private void SpawnUnits()
        {
            _unitsController.SpawnUnitsAtPoints(_viewSpawner.ActiveSpawnPoints);
        }
        
        private void OnBuilderUpgrade()
        {
            if (_isDestroyed)
                return;
            Level++;
            Delay(UpdateToCurrentLevel,.5f);
        }
        
        private void SetupUnloadPoints(ITowerBlocksBuilder builder)
        {
            _gridOutPusher = new TowerGridOutPusher(builder, transform);
            _unloadPointsManager = new UnloadPointsManager(builder, GlobalConfig.TowerUnloadPointsOffset);
            _unloadPointsManager.AddToAllLevels();
        }

        private void AddProgressS()
        {
            if (Level >= _settings.MaxLevelInd)
                return;
            _progBar.SetCount(_builder.Stored, _builder.Area);
        }
        
        private void DieFromDamage(IDamageable target)
        {
            _damageable.OnDied -= DieFromDamage;
            Kill();
        }

        private void UpdateToCurrentLevel()
        {
            CLog.LogWhite($"[Tower {gameObject.name}] Upgraded Lvl_{Level}");
            _viewSpawner.UpdateToLevel(Level);
            var levelSettings = _settings.levelSettings[Level];
            _damageable.SetMaxHealth(levelSettings.towerHealth);
            _unitsController.UpdateSettings(levelSettings);
            _progBar.Off();
            _acceptArea.SetSquareToLevel(Level);
            if (Level == _settings.MaxLevelInd)
            {
                _canUpgrade = false;
                _progBar.SetCount( _builder.Area, _builder.Area);
                _buildingAnimator.Hide();
            }
            else
            {
                if (Level == 1)
                {
                    _unitsController.ActivateAttack();
                    _radius.Show();
                    _flag.Hide();
                }
                _builder.BuildArea(levelSettings.buildingSettings, Level);
                _gridOutPusher.CheckAndPushOut();
                _unloadPointsManager.AddLatestPoints();
                _progBar.SetCount( _builder.Stored, _builder.Area);
                _buildingAnimator.ShowAt(_builder);
            }
            _radius.UpdateRadius(levelSettings.radius);
            if (Level >= 1)
            {
                _damagePoints.AddPoints(_viewSpawner.Views[^1].DamagePoints);
                _damageable.CanDamage = true;
            }
            
            if (_catapultSpawner.CheckCondition(Level))
                _catapultSpawner.SpawnCatapult(Team);
        }
        
    }
}