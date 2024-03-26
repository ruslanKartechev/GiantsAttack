using UnityEngine;

namespace GiantsAttack
{
    public interface IEnemyThrowWeapon
    {
        GameObject GameObject { get; }
        IThrowable Throwable { get; }
        ITarget Target { get; }
        IHealth Health { get; }
    }
}