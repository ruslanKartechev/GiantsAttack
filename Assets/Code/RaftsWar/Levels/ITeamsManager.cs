using System.Collections.Generic;
using RaftsWar.Boats;

namespace RaftsWar.Levels
{
    public interface ITeamsManager
    {
        public IList<EnemyTeam> EnemyTeams { get; }
        public Team PlayerTeam { get; }
        public BoatPartsManager PartsManager { get; }
        public BoatPlayer PlayerBoat { get; }
    }
}