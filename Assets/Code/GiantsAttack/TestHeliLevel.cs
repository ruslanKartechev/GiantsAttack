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
        private IHelicopter _player;
        private IHitCounter _hitCounter;
        private IControlsUI _controlsUI;
        
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
            
            var spawner = new HelicopterSpawner();
            var player = spawner.SpawnAt(_playerSpawnPoint, transform);
            player.Init(_initArgs);
            player.CameraPoints.SetCameraToOutside();
            player.CameraPoints.MoveCameraToInside(OnCameraSet);
            player.Aimer.BeginAim();
            player.Shooter.BeginShooting();
        }

        private void OnCameraSet()
        {
            
        }
    }
}