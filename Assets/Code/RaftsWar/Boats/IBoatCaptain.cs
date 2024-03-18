using UnityEngine;

namespace RaftsWar.Boats
{
    public interface IBoatCaptain
    {
        void Rotate(Vector3 moveDirection);
        void SetView(UnitViewSettings viewSettings);
        void DieRagdoll();
        void OnControlRelease();
    }
}