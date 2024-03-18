using System;
using RaftsWar.UI;
using UnityEngine;

namespace RaftsWar.Boats
{
    public interface ITeamPlayer : ITeamMember
    {
        event Action<ITeamPlayer> OnDied;
        bool IsDead { get; }
        ITarget Target { get; }
        Transform DeadCameraPoint { get; }
        void StopPlayer();
        void ActivatePlayer();
        void Kill();
        void SetTeamUnitUI(ITeamUnitUI ui);
    }
}