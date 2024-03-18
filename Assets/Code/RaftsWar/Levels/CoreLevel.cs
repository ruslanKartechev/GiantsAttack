using RaftsWar.Boats;
using RaftsWar.UI;
using SleepDev;
using UnityEngine;
using RaftsWar.Cam;

namespace RaftsWar.Levels
{
    public class CoreLevel : Level
    {
        [SerializeField] private float _addedTowerProgress = .2f;
        [SerializeField] private bool _useUI;
        [SerializeField] private PlayerCameraPointsSettingsSO _cameraPointsSettings;
        [SerializeField] private LevelTeamsManager _teamsManager;
        [SerializeField] private Transform _startCameraPoint;
        private IPlayerCamera _camera;
        private WinCondition _winCondition;
        private DestroyedCameraSetter _cameraSetter;
        private TeamKillMatcher _killMatcher;
        public PlayerCameraPointsSettingsSO CameraPointsSettings => _cameraPointsSettings;
        public override Transform StartCameraPoint => _startCameraPoint;
        
        public override void InitLevel()
        {
            var targetsManager = new TeamsTargetsManager();
            _camera = BoatUtils.GetCamera();
            _camera.SetPoint(_startCameraPoint);
            CLog.Log($"[MainLevel] Level Init");
            _teamsManager.SpawnAll(() =>
            {
                if (_useUI)
                    ShowStartMenu();
                else
                    Activate();
            }, _cameraPointsSettings.settings);
        }

        private void Activate()
        {
            var start = GCon.UIFactory.GetStartMenu();
            if(start != null)
                start.Off();
            var ui = (IGameplayMenu)GCon.UIFactory.GetGameplayMenu();
            _teamsManager.PlayerBoat.SetPlayerUI(ui);
            _teamsManager.PlayerTeam.Tower.UIDamagedEffect = ui.DamagedEffect;
            _teamsManager.PlayerTeam.Tower.UseUIDamageEffect = true;
            
            TeamsTargetsManager.Inst.ActivateTowers();
            TeamsTargetsManager.Inst.ActivateAllActors();
            LevelUtils.SetupNames(_teamsManager, ui);
            _winCondition = new WinCondition(_teamsManager, OnWin, OnFail);
            _cameraSetter = new DestroyedCameraSetter(_teamsManager, _camera);
            _killMatcher = new TeamKillMatcher(_teamsManager);
            LevelUtils.SendEventLevelStart();
        }

        private void ShowStartMenu()
        {
            var startMenu = (IMenuStart)GCon.UIFactory.GetStartMenu();
            startMenu.Show(Activate, ()=>{});
        }

        private void OnWin()
        {
            CLog.LogGreen($"[Level] ON WIN");
            TeamsTargetsManager.Inst.StopAllActors();
            LevelUtils.SendEventLevelWin();
            Delay(() =>
            {
                LevelUtils.CallWinScreen(_addedTowerProgress);
            }, 1.5f);
        }

        private void OnFail()
        {
            CLog.LogGreen($"[Level] ON FAIL");
            TeamsTargetsManager.Inst.StopAllActors();
            LevelUtils.SendEventLevelFail();
            Delay(LevelUtils.CallFailScreen, 1.5f);
        }
    }
}