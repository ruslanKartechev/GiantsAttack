using System.Collections.Generic;
using GameCore.Cam;
using GameCore.UI;
using UnityEngine;
using GameCore.Core;
using SleepDev;

namespace GiantsAttack
{
    public class TestHeliLevel : GameCore.Levels.Level, IStageResultListener
    {
        [SerializeField] private bool _useStartUi = true;
        [SerializeField] private HelicopterInitArgs _initArgs;
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private MonsterController _monster;
        [SerializeField] private List<LevelStage> _stages;
        [SerializeField] private LevelFinalSequence _finalSequence;
        private PlayerCamera _camera;
        private int _stageIndex = 0;
        private bool _isFinalizing;

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

        public override void Init()
        {
            _camera = CameraContainer.PlayerCamera as PlayerCamera;
            _controlsUI = GCon.UIFactory.GetControlsUI();
            _hitCounter = new PlayerHitCounter();
            _initArgs.hitCounter = _hitCounter;
            _initArgs.camera = _camera;
            _initArgs.controlsUI = _controlsUI;
            _gameplayMenu = GCon.UIFactory.GetGameplayMenu() as IGameplayMenu;
            _initArgs.aimUI = _gameplayMenu.AimUI;
            // init player
            SpawnAndInitPlayer();
            InitEnemy();
            foreach (var st in _stages)
                InitStage(st);
            _player.CameraPoints.SetCameraToOutside();
            StartTiming();
            _player.Mover.Loiter();
            if (_useStartUi)
                ShowStartUI();
            else
                BeginGameplay();
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
            StopTiming();
            var utils = new LevelUtils();
            var level = GCon.PlayerData.LevelTotal+1;
            utils.SendFailEvent(level, _timePassed, _hitCounter);
            utils.CallFailScreen(level);
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
            if (_isFinalizing)
                return;
            _isFinalizing = true;
            CLog.LogGreen($"{gameObject.name} LaunchFinalSequence");
            _finalSequence.Enemy = _monster;
            _finalSequence.Player = _player;
            _finalSequence.Camera = _camera;
            _finalSequence.Begin(Win);
            _gameplayMenu.Hide(() => {});
        }

        private void ShowStartUI()
        {
            var ui = GCon.UIFactory.GetStartMenu() as IMenuStart;
            ui.Show(OnStartLevel, () => {});
        }

        private void OnStartLevel()
        {
            GCon.UIFactory.GetStartMenu().Hide(() => {});
            _player.CameraPoints.MoveCameraToInside(BeginGameplay);
        }
        
        private void SpawnAndInitPlayer()
        {
            _initArgs.enemyTransform = _monster.Point;
            var spawner = new HelicopterSpawner();
            var player = spawner.SpawnAt(_playerSpawnPoint, _playerSpawnPoint.parent);
            player.Init(_initArgs);
            player.Shooter.DamageHitsUI = _gameplayMenu.DamageHits;
            _player = player;
        }

        private void InitEnemy()
        {
            _monster.Init(_player.BodySectionsUI);
        }

        private void BeginGameplay()
        {
            _stages[_stageIndex].Activate();
        }

        private void InitStage(LevelStage stage)
        {
            stage.Player = _player;
            stage.Camera = _camera;
            stage.Enemy = _monster;
            stage.UI = _gameplayMenu;
            stage.ResultListener = this;
        }

        private void NextStage()
        {
            if (_isCompleted)
                return;
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
            if (_isCompleted)
                return;
            NextStage();
        }

        public void OnStageFail(LevelStage stage)
        {
            Fail();
        }

        public void OnMainEnemyDead()
        {
            LaunchFinalSequence();
        }

        private void OnAllStagesPassed()
        {
            CLog.LogGreen($"{gameObject.name} All stages passed");
            LaunchFinalSequence();
        }
    }
}