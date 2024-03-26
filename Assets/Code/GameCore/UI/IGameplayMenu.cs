using SleepDev;

namespace GameCore.UI
{
    public interface IGameplayMenu : IUIScreen
    {
        JoystickUI JoystickUI { get; }
        IUIDamagedEffect DamagedEffect { get; }
        IAimUI AimUI { get; }
        IDamageHitsUI DamageHits { get; }
        EvadeUI EvadeUI { get; }
        IShootAtTargetUI ShootAtTargetUI { get; }
        IBodySectionsUI EnemyBodySectionsUI { get; }
    }
}