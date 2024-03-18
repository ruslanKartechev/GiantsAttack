using UnityEngine;

namespace RaftsWar.Boats
{
    public interface IDamagePointsProvider
    {
        public Transform GetClosestTarget(Vector3 sourcePoint);
        public Transform GetRandomTarget();
        
    }
}