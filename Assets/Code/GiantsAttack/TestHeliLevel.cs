using GameCore.Cam;
using GameCore.UI;
using UnityEngine;
using GameCore.Core;

namespace GiantsAttack
{
    public class TestHeliLevel : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private PlayerCamera _camera;
        [SerializeField] private HelicopterInitArgs _initArgs;
        
        [SerializeField] private Transform _lookAtTarget;
        [SerializeField] private CircularPathBuilder _pathBuilder;
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

            var spawner = new HelicopterSpawner();
            var player = spawner.SpawnAt(_playerSpawnPoint, transform);
            _player = player;
            player.Init(_initArgs);
            player.Aimer.AimUI = _gameplayMenu.AimUI;
            player.Shooter.DamageHitsUI = _gameplayMenu.DamageHits;
            
            player.CameraPoints.SetCameraToOutside();
            player.CameraPoints.MoveCameraToInside(OnCameraSet);
        }

        private void OnCameraSet()
        {
            _player.Mover.SetPath(_pathBuilder.Path, _lookAtTarget);
            _player.Mover.BeginMovement();
            _player.Aimer.BeginAim();
        }
    }
}