using UnityEngine;

namespace RaftsWar.Boats
{
    public interface ICatapult 
    {
        void Init(Team team);
        void Hide();
        void Show(bool animate);
        void SetPosition(Transform point);
        IUnloadPointsManager UnloadPointsManager { get; }
        IBoatPartReceiver BoatPartReceiver { get; }
    }
}