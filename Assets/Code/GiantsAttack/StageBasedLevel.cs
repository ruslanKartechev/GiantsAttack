using System.Collections.Generic;
using GameCore.Cam;
using GameCore.UI;
using UnityEngine;
using GameCore.Core;
using SleepDev;

namespace GiantsAttack
{
    public class StageBasedLevel : GameCore.Levels.Level, IStageResultListener
    {
        [SerializeField] private string _objective = "SAVE THE CITY";
        [SerializeField] private float _enemyHealth = 1000;
        [SerializeField] private float _moveAnimationSpeed = .8f;
        [SerializeField] private EnemyID _enemyID;
        [SerializeField] private EnemyView _enemyView;
        [SerializeField] private AimerSettingsSo _aimerSettings;
        [SerializeField] private HelicopterInitArgs _initArgs;
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private List<LevelStage> _stages;
        [SerializeField] private LevelStartSequence _startSequence;
        [SerializeField] private LevelFinalSequence _finalSequence;
        [SerializeField] private LevelFailSequence _failSequence;
        [SerializeField] private GameObject _playerMoverGo;
        private PlayerCamera _camera;
        private int _stageIndex = 0;
        private bool _isFinalizing;
        private byte _startReadyStage;

        private IMonster _enemy;
        private IPlayerMover _playerMover;
        private IHelicopter _player;
        private IHitCounter _hitCounter;
        private IControlsUI _controlsUI;
        private IGameplayMenu _gameplayMenu;
        
        #if UNITY_EDITOR
        private LevelDebugger _debugger;
        #endif
        
        
        private void Start()
        {
#if UNITY_EDITOR
            _debugger = gameObject.AddComponent<LevelDebugger>();
            _debugger.level = this;
#endif
        }

#if UNITY_EDITOR
        public void E_Init()
        {
            if (_startSequence == null)
            {
                _startSequence = FindObjectOfType<LevelStartSequence>();
                _startSequence?.E_Init();
            }

            if (_finalSequence == null)
            {
                _finalSequence = FindObjectOfType<LevelFinalSequence>();
                _finalSequence?.E_Init();
            }

            if (_failSequence == null)
                _failSequence = FindObjectOfType<LevelFailSequence>();

            if (_enemy == null)
                _enemy = FindObjectOfType<MonsterController>();

            if (_playerMoverGo == null)
            {
                var chance = FindObjectOfType<PlayerMover>();
                if (chance != null)
                    _playerMoverGo = chance.gameObject;
                else
                {
                    var chance2 = FindObjectOfType<PlayerAroundMover>();
                    if(chance2 != null)
                        _playerMoverGo = chance2.gameObject;
                }
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif


        public override void Init()
        {
            GCon.DataSaver.Save();
            _playerMover = _playerMoverGo.GetComponent<IPlayerMover>();
            _camera = CameraContainer.PlayerCamera as PlayerCamera;
            _controlsUI = GCon.UIFactory.GetControlsUI();
            _hitCounter = new PlayerHitCounter();
            _initArgs.hitCounter = _hitCounter;
            _initArgs.camera = _camera;
            _initArgs.controlsUI = _controlsUI;
            _initArgs.aimerSettings = _aimerSettings.aimerSettings;
            _gameplayMenu = GCon.UIFactory.GetGameplayMenu() as IGameplayMenu;
            _gameplayMenu.Off();
            _initArgs.aimUI = _gameplayMenu.AimUI;
            // spawning
            SpawnEnemy();
            SpawnPlayer();
            // init
            InitPlayer();
            InitEnemy();
            _player.CameraPoints.SetCameraToOutside();
            // player mover
            _playerMover.Player = _player;
            _playerMover.Enemy = _enemy;
            _player.Mover.Loiter();
            // stages
            foreach (var st in _stages)
                InitStage(st);
            StartTiming();
            _startSequence.Enemy = _enemy;
            _startSequence.Begin(OnStartSequenceFinished);
            ShowStartUI();
        }

        public override void Win()
        {
            CLog.LogGreen($"{gameObject.name} Win call");
            if (_isCompleted)
                return;
            _isCompleted = true;
            StopTiming();
            var utils = new LevelUtils();
            var level = GCon.PlayerData.LevelTotal+1;
            utils.SendWinEvent(level, _timePassed, _hitCounter);
            utils.CallWinScreen(level);
        }

        public override void Fail()
        {
            if (_isCompleted)
                return;
            _isCompleted = true;
            StopTiming();
            var utils = new LevelUtils();
            var level = GCon.PlayerData.LevelTotal+1;
            _failSequence.Player = _player;
            _failSequence.Enemy = _enemy;
            _failSequence.Play(() =>
            {
                utils.SendFailEvent(level, _timePassed, _hitCounter);
                utils.CallFailScreen(level);
            });
        }
        
        public override void Pause()
        {
            Time.timeScale = 0f;
        }

        public override void Resume()
        {
            Time.timeScale = 1f;
        }

        private void LaunchFinalSequence()
        {
            if (_isFinalizing) return;
            _isFinalizing = true;
            CLog.LogGreen($"{gameObject.name} LaunchFinalSequence");
            _finalSequence.Enemy = _enemy;
            _finalSequence.Player = _player;
            _finalSequence.PlayerMover = _playerMover;
            _finalSequence.Camera = _camera;
            _finalSequence.Begin(Win);
            _gameplayMenu.Hide(() => {});
        }

        private void ShowStartUI()
        {
            var ui = GCon.UIFactory.GetStartMenu() as IMenuStart;
            ui.Show(OnStartLevel, () =>
            {
                ui.ShowObjective(() => {});
            }, _objective);
        }

        private void OnStartLevel()
        {
            GCon.UIFactory.GetStartMenu().Hide(() => {});
            _gameplayMenu.Show(() => {});
            _player.CameraPoints.MoveCameraToInside(OnCameraSet);
        }
        
        private void SpawnPlayer()
        {
            var spawner = new HelicopterSpawner();
            _player = spawner.SpawnAt(_playerSpawnPoint, _playerSpawnPoint.parent);
        }
        
        private void SpawnEnemy()
        {
            var spawner = GetComponent<IEnemySpawner>();
            _enemy = spawner.SpawnEnemy(_enemyID);
        }
        
        private void InitPlayer()
        {
            _initArgs.enemyTransform = _enemy.Point;
            _player.Init(_initArgs);
        }

        private void InitEnemy()
        {
            _enemy.Init(_gameplayMenu.EnemyBodySectionsUI , _enemyHealth, _enemyView);
            _enemy.SetMoveAnimationSpeed(_moveAnimationSpeed);
        }

        private void OnCameraSet()
        {
            _player.Shooter.Gun.PlayGunsInstallAnimation();
            _startReadyStage++;
            if(_startReadyStage == 2)
                BeginGameplay();
        }

        private void OnStartSequenceFinished()
        {
            _startReadyStage++;
            if(_startReadyStage == 2)
                BeginGameplay();
        }
        
        private void BeginGameplay()
        {
            _playerMover.Begin();
            _stages[_stageIndex].Activate();
        }

        private void InitStage(LevelStage stage)
        {
            stage.Player = _player;
            stage.Camera = _camera;
            stage.Enemy = _enemy;
            stage.UI = _gameplayMenu;
            stage.ResultListener = this;
            stage.PlayerMover = _playerMover;
        }

        private void NextStage()
        {
            if (_isCompleted) return;
            CLog.LogGreen($"[Level] Stage competed callback");
            _stageIndex++;
            if (_stageIndex >= _stages.Count)
            {
                OnAllStagesPassed();
                return;
            }
            _stages[_stageIndex].Activate();
        }
        
        public void OnStageComplete(LevelStage stage)
        {
            if (_isCompleted || _isFinalizing) return;
            NextStage();
        }

        
        // LEVEL RESULTS
        public void OnMainEnemyDead()
        {
            if (_isCompleted || _isFinalizing) return;
            _playerMover.Pause(false);
            LaunchFinalSequence();
        }
        
        public void OnStageFail(LevelStage stage)
        {
            if (_isCompleted || _isFinalizing) return;
            Fail();
        }
        
        private void OnAllStagesPassed()
        {
            if (_isCompleted || _isFinalizing) return;
            CLog.LogGreen($"{gameObject.name} All stages passed");
            _playerMover.Pause(false);
            LaunchFinalSequence();
        }
    }
}