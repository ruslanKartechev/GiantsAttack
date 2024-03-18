using UnityEngine;

namespace RaftsWar.Boats
{
    public interface IUnloadPointsManager
    {
        Vector3 GetClosestPoint(Vector3 closestTo);
    }
}