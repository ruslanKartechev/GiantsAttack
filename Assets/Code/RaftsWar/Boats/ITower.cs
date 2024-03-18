using System;
using RaftsWar.UI;
using UnityEngine;

namespace RaftsWar.Boats
{
    public interface ITower : IBoatPartReceiver, ITarget
    {
        event Action<ITower> OnDestroyed; 
        void Init(Team team, int startLevel = 0);
        IUnloadPointsManager UnloadPointsManager { get; }
        Transform DestroyedCameraPoint { get; }
        bool IsDestroyed { get; }
        void Activate();
        void Stop();
        void Kill();
        ICatapult Catapult { get; }
        int Level { get; }
        bool IsMaxLevel { get; }
        bool UseUIDamageEffect { get; set; }
        IUIDamagedEffect UIDamagedEffect { get; set; }
    }
    
}