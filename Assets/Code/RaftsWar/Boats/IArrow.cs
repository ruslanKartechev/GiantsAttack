using UnityEngine;

namespace RaftsWar.Boats
{
    public interface IArrow
    {
        GameObject Go { get; }
        void SetView(UnitViewSettings viewSettings);
        void Launch(float speed, ITarget target, DamageDealer damageDealer);
        void Hide();
        void Reset();
    }
    
    
}