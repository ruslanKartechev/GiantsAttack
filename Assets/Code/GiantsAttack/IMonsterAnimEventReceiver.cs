using System;

namespace GiantsAttack
{
    public interface IMonsterAnimEventReceiver
    {
        event Action OnPickup;
        event Action OnThrow;
        
    }
}