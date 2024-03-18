using UnityEngine;

namespace RaftsWar.Boats
{
    public interface ITarget : ITeamMember
    {
        Transform Point { get; }
        IDamageable Damageable { get;}
        IArrowStuckTarget ArrowStuckTarget { get;}
        IDamagePointsProvider DamagePointsProvider { get; }
 
    }
}