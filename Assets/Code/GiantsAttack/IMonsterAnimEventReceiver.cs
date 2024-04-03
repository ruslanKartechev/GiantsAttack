using System;

namespace GiantsAttack
{
    public interface IMonsterAnimEventReceiver
    {
        event Action OnPickup;
        event Action OnThrow;
        event Action OnPunch;
        event Action OnPunchBegan;
        event Action OnAnimationOver;
        event Action OnJumpDown;
    }
}