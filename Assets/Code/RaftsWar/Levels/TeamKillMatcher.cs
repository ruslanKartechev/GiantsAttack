using RaftsWar.Boats;

namespace RaftsWar.Levels
{
    /// <summary>
    /// Is used to kill Tower if Boat is dead and vice versa
    /// Uses OnDied and OnDestroyed events of ITeamPlayer and ITower 
    /// </summary>
    public class TeamKillMatcher
    {
        private LevelTeamsManager _teams;

        public TeamKillMatcher(LevelTeamsManager teamsManager)
        {
            _teams = teamsManager;
            foreach (var enemyTeam in teamsManager.EnemyTeams)
            {
                enemyTeam.EnemyBoat.OnDied += OnBoatDied;
                enemyTeam.Tower.OnDestroyed += OnTowerDied;
            }
        }

        private void OnTowerDied(ITower tower)
        {
            tower.Team.Player.OnDied -= OnBoatDied;
            tower.OnDestroyed -= OnTowerDied;
            tower.Team.Player.Kill();
        }

        private void OnBoatDied(ITeamPlayer player)
        {
            player.OnDied -= OnBoatDied;
            player.Team.Tower.OnDestroyed -= OnTowerDied;
            player.Team.Tower.Kill();
        }
    }
}