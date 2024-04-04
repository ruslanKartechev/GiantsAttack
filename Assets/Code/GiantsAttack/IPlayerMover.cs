using System;
using SleepDev;

namespace GiantsAttack
{
    public interface IPlayerMover
    {
        IHelicopter Player { get; set; }
        void Pause(bool loiter);
        void Resume();
        void Evade(EDirection2D direction2D, Action callback, float distance);
        void Begin();
    }
}