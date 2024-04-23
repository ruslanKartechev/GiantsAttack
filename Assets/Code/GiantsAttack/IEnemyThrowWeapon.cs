using UnityEngine;

namespace GiantsAttack
{
    public interface IEnemyThrowWeapon
    {
        GameObject GameObject { get; }
        IThrowable Throwable { get; }
        IHealth Health { get; }
        AnimatedTarget AnimatedVehicle { get; }
    }
}