using System.Collections;
using GameCore;
using GameCore.Cam;
using GameCore.UI;
using UnityEngine;
using GameCore.Core;
using SleepDev;

namespace GiantsAttack
{
    public class TutorialLevel : Level, IStageResultListener
    {
        [SerializeField] private float _enemyHealth = 1000;
        [SerializeField] private float _moveAnimationSpeed = .8f;
        [SerializeField] private EnemyID _enemyID;
        [SerializeField] private EnemyView _enemyView;
        [SerializeField] private AimerSettingsSo _aimerSettings;
        [SerializeField] private HelicopterInitArgs _initArgs;
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private LevelStageHavok _havokStage;
        [SerializeField] private LevelStartSequence _startSequence;
        [SerializeField] private LevelFinalSequence _finalSequence;
        [SerializeField] private LevelFailSequence _failSequence;
        [SerializeField] private GameObject _playerMoverGo;
        [Space(10)]
        [SerializeField] private TutorialUI _tutorUI;
        [SerializeField] private Transform _startCamera;
        [SerializeField] private float _cameraMoveDelay = 1f;
        [SerializeField] private float _aimAnimDelay;
        [SerializeField] private float _titleDelay;
        [SerializeField] private float _aimTutorDelay;
        [SerializeField] private float _hideTutorDelay;

        private PlayerCamera _camera;
        private bool _isFinalizing;
        private byte _startReadyStage;

        private IMonster _enemy;
        private IPlayerMover _playerMover;
        private IHelicopter _player;
        private IHitCounter _hitCounter;
        private IControlsUI _controlsUI;
        private IGameplayMenu _gameplayMenu;
        private LevelUtils _levelUtils;
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
            GCon.DataSaver.Save();
            DamageCalculator.Clear();

            _levelUtils = new LevelUtils(_player, _hitCounter);
            _levelUtils.SendStartEvent(GCon.PlayerData.LevelTotal + 1);
            
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
            // player mover
            _playerMover.Player = _player;
            _playerMover.Enemy = _enemy;
            _player.Mover.Loiter();
            // stages
            InitStage(_havokStage);
            StartTiming();
            _startSequence.Enemy = _enemy;
            //
            StartCoroutine(Starting());

        }

        public override void Win()
        {
            CLog.LogGreen($"{gameObject.name} Win call");
            if (_isCompleted)
                return;
            _isCompleted = true;
            StopTiming();
            var level = GCon.PlayerData.LevelTotal+1;
            _levelUtils.SendWinEvent(level, _timePassed, _hitCounter);
            _levelUtils.CallWinScreen(level);
        }

        public override void Fail()
        {
            if (_isCompleted)
                return;
            _isCompleted = true;
            StopTiming();
            var level = GCon.PlayerData.LevelTotal+1;
            _failSequence.Player = _player;
            _failSequence.Enemy = _enemy;
            _failSequence.Play(() =>
            {
                _levelUtils.SendFailEvent(level, _timePassed, _hitCounter);
                _levelUtils.CallFailScreen(level);
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
            if (_isFinalizing)
                return;
            _isFinalizing = true;
            CLog.LogGreen($"{gameObject.name} LaunchFinalSequence");
            _finalSequence.Enemy = _enemy;
            _finalSequence.Player = _player;
            _finalSequence.PlayerMover = _playerMover;
            _finalSequence.Camera = _camera;
            _finalSequence.Begin(Win);
            _gameplayMenu.Hide(() => {});
        }

        private void OnStartLevel()
        {
            GCon.UIFactory.GetStartMenu().Hide(() => {});
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
        

        private void InitStage(LevelStage stage)
        {
            stage.Player = _player;
            stage.Camera = _camera;
            stage.Enemy = _enemy;
            stage.UI = _gameplayMenu;
            stage.ResultListener = this;
            stage.PlayerMover = _playerMover;
        }

        public void OnStageComplete(LevelStage stage)
        {
            if (_isCompleted || _isFinalizing)
                return;
            OnAllStagesPassed();
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
        // // //

        private void OnStartSequence()
        { }

        private bool _camSet;
        private bool _aimStarted;
        
        private void OnCameraSet() => _camSet = true;
        private void OnInputBtnDown() => _aimStarted = true;
        
        private IEnumerator Starting()
        {
            _camera.SetPoint(_startCamera);
            _startSequence.Begin(OnStartSequence);
            CLog.LogBlue($"[Tutor] Started");
            yield return new WaitForSeconds(_aimAnimDelay);
            _tutorUI.ShowSquareAim();
            yield return new WaitForSeconds(_titleDelay);
            CLog.LogBlue($"[Tutor] Title");
            _tutorUI.TextPrinter.PrintText();
            yield return new WaitForSeconds(_cameraMoveDelay);
            _tutorUI.HideAim();
            CLog.LogBlue($"[Tutor] Camera movement");
            _player.CameraPoints.MoveCameraToInside(OnCameraSet);
            _gameplayMenu.On();
            _controlsUI.Off();
            while (_camSet == false)
                yield return null;
            CLog.LogBlue($"[Tutor] Camera set, gun install, stage activated");
            _tutorUI.TextPrinter.HideAnimated();
            _player.Shooter.Gun.PlayGunsInstallAnimation();
            _havokStage.Activate();
            _player.Aimer.StopAim();
            _player.Shooter.StopShooting();
            _controlsUI.Off();
            yield return new WaitForSeconds(_aimTutorDelay);
            CLog.LogBlue($"[Tutor] Aiming tutor");
            Time.timeScale = 0f;
            _tutorUI.ShowHandAreaAnimated();
            _tutorUI.InputBtn.OnDown += OnInputBtnDown;
            while (_aimStarted == false)
                yield return null;
            Time.timeScale = 1f;
            _controlsUI.On();
            _tutorUI.InputBtn.OnDown -= OnInputBtnDown;
            _tutorUI.InputBtn.gameObject.SetActive(false);
            _player.Aimer.BeginAim();
            _playerMover.Begin();
            yield return new WaitForSeconds(_hideTutorDelay);
            _tutorUI.HideHandAreaAnimated();
        }
    }
}