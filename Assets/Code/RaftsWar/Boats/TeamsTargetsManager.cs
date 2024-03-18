using System.Collections.Generic;

namespace RaftsWar.Boats
{
    /// <summary>
    /// Holds references to all towers, players and boat-part targets.
    /// ! Supposed to be created before Towers and Boats are spawned !
    /// Supposed to be used for target search for tower-units and catapults
    /// </summary>
    public class TeamsTargetsManager
    {
        private static TeamsTargetsManager _inst;
        public static TeamsTargetsManager Inst => _inst;

        private List<ITarget> _targets;
        private List<ITeamPlayer> _players;
        private List<ITower> _towers;
        /// <summary>
        /// Damageable targets for towers and catapults. Includes BoatParts and Towers. NOT PLayers
        /// </summary>
        public List<ITarget> Targets => _targets;
        /// <summary>
        /// OnlyIncludesPlayers
        /// </summary>
        public List<ITeamPlayer> Players => _players;
        /// <summary>
        /// Only Includes Towers. All Towers are also in Targets
        /// </summary>
        public List<ITower> Towers => _towers;

        public TeamsTargetsManager()
        {
            _inst = this;
            _targets = new List<ITarget>(20);
            _players = new List<ITeamPlayer>(5);
            _towers = new List<ITower>(5);
        }

        public void AddPlayer(ITeamPlayer player)
        {
            _players.Add(player);
        }
        
        public void RemovePlayer(ITeamPlayer player)
        {
            _players.Remove(player);
        }

        public void AddTarget(ITarget target)
        {
            _targets.Add(target);
        }

        public void RemoveTarget(ITarget target)
        {
            _targets.Remove(target);
        }

        public void AddTower(ITower tower)
        {
            _towers.Add(tower);
            _targets.Add(tower);
            tower.OnDestroyed += OnTowerDestroyed;
        }
        
        public void RemoveTower(ITower tower)
        {
            _towers.Remove(tower);
            _targets.Remove(tower);
        }

        public void StopAllActors()
        {
            foreach (var player in _players)
                player.StopPlayer();
        }

        public void ActivateAllActors()
        {
            foreach (var player in _players)
                player.ActivatePlayer();
        }
        
        public void ActivateTowers()
        {
            foreach (var target in _towers)
                target.Activate();
        }
        
        public void StopTowers()
        {
            foreach (var target in _towers)
                target.Stop();
        }
        
        private void OnTowerDestroyed(ITower tower)
        {
            tower.OnDestroyed -= OnTowerDestroyed;
            RemoveTower(tower);
        }
    }
}