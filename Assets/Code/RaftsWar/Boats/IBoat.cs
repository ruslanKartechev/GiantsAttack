using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    public interface IBoat
    {
        Team Team { get; set; }
        IDamageable DamageTarget { get; set; }
        bool MoveBoat(Vector3 direction);
        BoatPart RootPart { get; }
        IList<BoatPart> Parts { get; }
        void PushOutFromBlockedArea(Square outOfSquare, Transform root);
        void HandleCollisionPushback(IBoat anotherBoat);
        void PlayDamaged();
        
    }
}