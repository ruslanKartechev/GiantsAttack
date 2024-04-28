using System;

namespace GiantsAttack
{
    public interface IMonsterAnimEventReceiver
    {
        event Action EOnPickup;
        event Action EOnThrow;
        event Action EOnPunch;
        event Action EOnPunchBegan;
        event Action EOnAnimationOver;
        event Action EOnJumpDown;
    }
}