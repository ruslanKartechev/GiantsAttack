using System.Collections.Generic;
using GameCore.Cam;
using GameCore.UI;
using UnityEngine;
using GameCore.Core;
using SleepDev;

namespace GiantsAttack
{
    public class TestHeliLevel : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private PlayerCamera _camera;
        [SerializeField] private HelicopterInitArgs _initArgs;
        [SerializeField] private LevelStage _stage1;
        [SerializeField] private MonsterController _monster;
        
        private IHelicopter _player;
        private IHitCounter _hitCounter;
        private IControlsUI _controlsUI;
        private IGameplayMenu _gameplayMenu;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            _controlsUI = GCon.UIFactory.GetControlsUI();
            _hitCounter = new PlayerHitCounter();
            _initArgs.hitCounter = _hitCounter;
            _initArgs.camera = _camera;
            _initArgs.controlsUI = _controlsUI;
            _gameplayMenu = GCon.UIFactory.GetGameplayMenu() as IGameplayMenu;
            // init player
            SpawnAndInitPlayer();
            InitEnemy();
            InitStage(_stage1);
            _player.CameraPoints.SetCameraToOutside();
            _player.CameraPoints.MoveCameraToInside(OnCameraSet);
        }

        private void SpawnAndInitPlayer()
        {
            var spawner = new HelicopterSpawner();
            var player = spawner.SpawnAt(_playerSpawnPoint, transform);
            player.Init(_initArgs);
            player.Aimer.AimUI = _gameplayMenu.AimUI;
            player.Shooter.DamageHitsUI = _gameplayMenu.DamageHits;
            _player = player;
        }

        private void InitEnemy()
        {
            _monster.Init();
        }

        private void OnCameraSet()
        {
            _stage1.Activate();
        }

        private void InitStage(LevelStage stage)
        {
            stage.Player = _player;
            stage.Camera = _camera;
            stage.Enemy = _monster;
            stage.CompletedCallback = OnStageCompleted;
            stage.UI = _gameplayMenu;
        }

        private void OnStageCompleted()
        {
            CLog.LogGreen($"[Level] Stage competed callback");
        }
    }
}