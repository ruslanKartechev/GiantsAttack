using RaftsWar.Boats;
using RaftsWar.Cam;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Levels
{
    /// <summary>
    /// Is used to move camera to a destroyed boat or tower
    /// </summary>
    public class DestroyedCameraSetter
    {
        private IPlayerCamera _camera;
        private BoatPlayer _playerBoat;
        private LevelTeamsManager _teams;
        
        public DestroyedCameraSetter(LevelTeamsManager teamsManager, IPlayerCamera camera)
        {
            _teams = teamsManager;
            _camera = camera;
            _playerBoat = _teams.PlayerBoat;
            foreach (var team in teamsManager.EnemyTeams)
            {
                team.Tower.OnDestroyed += OnTowerDied;
                team.EnemyBoat.OnDied += OnBoatDied;
            }
            teamsManager.PlayerTeam.Tower.OnDestroyed += OnTowerDied;
        }

        private void OnBoatDied(ITeamPlayer player)
        {
            player.OnDied -= OnBoatDied;
            player.Team.Tower.OnDestroyed -= OnTowerDied;
            if(player.Team.Tower.IsDestroyed == false)
                MoveCamera(player.DeadCameraPoint);
        }

        private void OnTowerDied(ITower tower)
        {
            // CLog.LogBlue($"[LevelCamManager] Tower has been destroyed");
            tower.OnDestroyed -= OnTowerDied;
            tower.Team.Player.OnDied -= OnBoatDied;
            if(tower.Team.Player.IsDead == false)
                MoveCamera(tower.DestroyedCameraPoint);
        }

        private void MoveCamera(Transform point)
        {
            _camera.AddCommand(new CameraCommandMoveToPoint(point, 
                GlobalConfig.ToTowerCameraMoveTime));
            _camera.AddCommand(new CameraCommandWait(GlobalConfig.ToTowerCameraWait,
                () =>
                {
                    if (_playerBoat.IsDead)
                        return;
                    _playerBoat.CameraPointsManger.ResetIndex();
                    _playerBoat.ReturnCameraToPlayer();
                }));
        }
        
    }
}