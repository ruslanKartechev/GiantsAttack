using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaftsWar.UI;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CreosTower : MonoExtended, ITower
    {
        [SerializeField] private int _levelToStartAutoUpgrading; 
        [SerializeField] private float _delayBetweenUpgrades = 2f;
        [SerializeField] private Team _team;
        [SerializeField] private BoatSettingsSo _boatSettings;
        [SerializeField] private PlayerCameraPointsSettingsSO _cameraPoints;
        [SerializeField] private BoatPlayer _playerBoat;
        [SerializeField] private RaftAcceptArea _raftAcceptArea;
        [SerializeField] private MenuGameplay _gameplayMenu;
        [Header("Enemies")]
        [SerializeField] private List<EnemyData> _enemies;
        [Space(10)]
        [SerializeField] private bool _useRadius;
        [SerializeField] private TowerRadius _radius;
        [Space(10)] 
        [SerializeField] private float _blockScale;
        [SerializeField] private Transform _hideToPoint;
        [SerializeField] private float _blockMoveTime;
        [Header("Building")]
        [SerializeField] private TowerBlocksBuilder _builder;
        [SerializeField] private TowerUnitsController _unitsController;
        [SerializeField] private TowerUnitsElectric _unitsElectric;
        [SerializeField] private List<UpgradeTarget> _targets;
        private float _afterUpgradeDelay;
        private int _receivedCount;
        private bool _canAccept = true;
        private int _areaIndex;
        private int _upgradeLevel;
        private bool _isAutoUpgrading;

        
        [System.Serializable]
        public class UpgradeTarget
        {
            [Header("Count for next upgrade")]
            public int targetCount;
            [Header("NULL if grid is not filled")]
            public BuildingBase buildingArea;
            public bool clearPrevUnits = true;
            public float afterSpawnDelay;
            public float towerRadius = 1;
            public List<TowerViewPart> hideTargets;
            public List<TowerViewPart> showTargets;

            public SquareGrid Grid { get; set; }
        }

        [System.Serializable]
        public class EnemyData
        {
            public Team team;
            public CreosEnemyBoat boat;
            public List<Transform> roamingPoints;

            public void Init()
            {
                boat.Init(team, roamingPoints);
            }
        }
        
        

        private void Start()
        {
            InitManagers();
            InitBuildingAreas();
            _raftAcceptArea.SetSquareToLevel(0);
            _team.CurrentPartReceiver = this;
            _radius.SetRadius(_targets[0].towerRadius);
            _unitsController.Radius = _targets[0].towerRadius;
            _radius.Hide();
            InitPlayer();
            InitEnemies();
        }

        private void InitPlayer()
        {
            _playerBoat.Init(_team, _boatSettings.settings, _cameraPoints.settings);
            _playerBoat.SetPlayerUI(_gameplayMenu);
            _gameplayMenu.NamesUIManager.AddPlayer(_playerBoat);
            _playerBoat.ActivatePlayer();
        }

        private void InitManagers()
        {
            var targetsManager = new TeamsTargetsManager();
            _unitsController.Tower = this;
            _unitsController.UpdateSettings(_team.TowerSettings.levelSettings[1]);
        }

        private void InitEnemies()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Init();
                enemy.boat.ActivatePlayer();
            }
        }
        
        private void InitBuildingAreas()
        {
            var gridSize = new Vector2Int(2, 2);
            _builder.BlockScale = _blockScale;
            _builder.BuildingSettings = _team.TowerSettings.levelSettings[1].buildingSettings;
            if (_targets[0].buildingArea != null)
            {
                var target = _targets[0];
                target.Grid = _builder.BuildGrid(gridSize, 2);
                target.buildingArea.SpawnTransparent(target.Grid, gridSize, _blockScale, 1f);
            }

            if (_targets[1].buildingArea != null)
            {
                var target = _targets[1];
                var centralGrid = _builder.BuildGrid(gridSize, 0);
                TowerBlocksBuilder.ModifyCentralGridByTowerLevel(centralGrid, 3);
                target.Grid = centralGrid;
                target.buildingArea.SpawnTransparent(target.Grid, gridSize, _blockScale, 1f);
            }

            if (_targets[2].buildingArea != null)
            {
                var target = _targets[2];
                target.Grid = _builder.BuildGrid(gridSize, 1);
                target.buildingArea.SpawnTransparent(target.Grid, gridSize, _blockScale, 1f);
            }

            if (_targets[3].buildingArea != null)
            {
                var target = _targets[3];
                target.Grid = _builder.BuildGrid(gridSize, 3);
                target.buildingArea.SpawnTransparent(target.Grid, gridSize, _blockScale, 1f);
            }         
   
        }


        public Team Team => _team;
        public bool CanTake() => _canAccept;
        public Square2D GetAreaSquare()
        {
            return _raftAcceptArea.CurrentSquare;
        }

        public void TakeBoatPart(BoatPart part)
        {
            // CLog.Log($"Take boat part {part.gameObject.name}");
            var raft = part.GetComponent<IBuildingBlock>();
            raft.Take();
            raft.SetScale(_blockScale);
            raft.SetYScale(1f);
            _receivedCount++;

            if (_isAutoUpgrading)
            {
                SendRaft(raft.Transform, _hideToPoint.localPosition, () => {
                    raft.Transform.gameObject.SetActive(false);
                });
                return;
            }
            
            var target = _targets[_upgradeLevel];
            // taking the raft
            if (target.buildingArea != null)
            {
                var indexPos = (_receivedCount-1) % 4;
                var posData = target.Grid.GetGridPos(indexPos);
                raft.SetSide(posData.side);
                SendRaft(raft.Transform, posData.position, _upgradeLevel, indexPos);
            }
            else
            {
                SendRaft(raft.Transform, _hideToPoint.localPosition, () => {
                    raft.Transform.gameObject.SetActive(false);
                });
            }
            // upgrading towers
            if (target.targetCount == _receivedCount)
            {
                if (_upgradeLevel == 0 && _useRadius)
                {
                    CLog.Log($"Show radius");
                    _radius.Show();
                }
                _upgradeLevel++;
                CLog.Log($"Upgrade level changed to {_upgradeLevel}");
                if (_upgradeLevel == _levelToStartAutoUpgrading)
                {
                    CLog.Log($"Auto upgrade level reached");
                    _isAutoUpgrading = true;
                    StartCoroutine(AutoUpgrading());
                }
                else
                {
                    if (_upgradeLevel >= _targets.Count)
                        _canAccept = false;
                    else
                    {
                        DelayCanTake();
                        _afterUpgradeDelay = target.afterSpawnDelay;
                    }
                }
                
                foreach (var view in target.hideTargets)
                    view.HideAnimated();
                var archerPoints = new List<Transform>(20);
                foreach (var view in target.showTargets)
                {
                    view.Show();
                    archerPoints.AddRange(view.SpawnPoints);   
                }
            
                if (target.clearPrevUnits)
                {
                    CLog.LogYellow("Clearing previous units");
                    _unitsController.StopAndClearSpawnedUnits();
                }
                CLog.Log($"Spawning units at {archerPoints.Count}");
                _unitsController.SpawnUnitsAtPoints(archerPoints);
                UpdateRadius(target.towerRadius);
                _unitsController.ActivateAttack();
            }
        }

        private IEnumerator AutoUpgrading()
        {
            _canAccept = true;
            while (_upgradeLevel <= _targets.Count - 1)
            {
                yield return new WaitForSeconds(_delayBetweenUpgrades);
                CLog.LogWhite($"Auto upgrading to level {_upgradeLevel}");
                var target = _targets[_upgradeLevel];
                foreach (var view in target.hideTargets)
                    view.HideAnimated();
                var archerPoints = new List<Transform>(20);
                foreach (var view in target.showTargets)
                {
                    view.Show();
                    archerPoints.AddRange(view.SpawnPoints);   
                }

                if (target.clearPrevUnits)
                {
                    CLog.LogYellow("Clearing previous units");
                    _unitsController.StopAndClearSpawnedUnits();
                }
                CLog.Log($"Spawning units at {archerPoints.Count}");
                _unitsController.SpawnUnitsAtPoints(archerPoints);
                UpdateRadius(target.towerRadius);
                _unitsController.ActivateAttack();
                _upgradeLevel++;
                yield return null;
            }
            CLog.LogGreen($"Reached maximum upgrade level");
            if (_unitsElectric != null)
            {
                CLog.Log($"Activating electric tower units");
                _unitsElectric.Tower = this;
                _unitsElectric.UpdateSettings(_team.TowerSettings.levelSettings[1]);
                _unitsElectric.Radius = _targets[^1].towerRadius;
                _unitsElectric.ActivateAttack();
            }
            
        }

        private void UpdateRadius(float val)
        {
            if(_useRadius)
                _radius.UpdateRadius(val);
            _unitsController.Radius = val;
        }

        private void SendRaft(Transform tr, Vector3 localPos, int targetindex, int posIndex)
        {
            SendRaft(tr, localPos, ()=>{
                _targets[targetindex].buildingArea.HideAt(posIndex);
            });
        }

        private void SendRaft(Transform tr, Vector3 localPos, Action onEnd)
        {
            tr.parent = transform;
            var ss = DOTween.Sequence();
            var p1 = tr.localPosition + Vector3.up * 5f;
            ss.Append(tr.DOLocalMove(p1, _blockMoveTime / 2f).SetEase(Ease.Linear));
            ss.Append(tr.DOLocalMove(localPos, _blockMoveTime).SetEase(Ease.Linear));
            ss.OnComplete(() => {
                onEnd?.Invoke();
            });
        }        

        private void DelayCanTake()
        {
            _canAccept = false;
            Delay(() => { _canAccept = true;}, _afterUpgradeDelay);
        }

        public Transform Point { get; }
        public IDamageable Damageable { get; }
        public IArrowStuckTarget ArrowStuckTarget { get; }
        public IDamagePointsProvider DamagePointsProvider { get; }
        public event Action<ITower> OnDestroyed;
        public void Init(Team team, int startLevel = 0)
        {
        }

        public IUnloadPointsManager UnloadPointsManager { get; }
        public Transform DestroyedCameraPoint { get; }
        public bool IsDestroyed => false;
        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Kill()
        {
            throw new NotImplementedException();
        }

        public ICatapult Catapult { get; }
        public int Level => 4;
        public bool IsMaxLevel => false;
        public bool UseUIDamageEffect { get; set; }
        public IUIDamagedEffect UIDamagedEffect { get; set; }
    }
}