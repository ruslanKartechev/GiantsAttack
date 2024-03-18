using System;
using System.Collections.Generic;
using RaftsWar.Boats;

namespace RaftsWar.Levels
{
    public class WinCondition
    {
        private List<ITower> _enemyTowers;
        private ITeamsManager _teamsManager;
        private Action _winCallback;
        private Action _failCallback;
        private bool _completed;
        

        public WinCondition(ITeamsManager teamsManager, Action winCallback, Action failCallback)
        {
            _winCallback = winCallback;
            _failCallback = failCallback;
            _teamsManager = teamsManager;
            _enemyTowers = new List<ITower>(_teamsManager.EnemyTeams.Count);
            foreach (var team in _teamsManager.EnemyTeams)
            {
                _enemyTowers.Add(team.Tower);
                team.Tower.OnDestroyed += OnTowerBroken;
            }
            teamsManager.PlayerBoat.OnDied += (t) => { Fail();};
            teamsManager.PlayerTeam.Tower.OnDestroyed += (t) => { Fail();};
        }

        private void OnTowerBroken(ITower tower)
        {
            if (_completed)
                return;
            _enemyTowers.Remove(tower);
            if (_enemyTowers.Count == 0)
                Win();
        }

        private void Fail()
        {
            if (_completed)
                return;
            _completed = true;
            _failCallback.Invoke();
        }

        private void OnTowerDestroyed(IDamageable tower)
        {
        
        }

        private void Win()
        {
            _completed = true;
            _winCallback.Invoke();
        }
    }
}