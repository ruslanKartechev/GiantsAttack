using GiantsAttack;

namespace GameCore.UI
{
    public interface IGameplayMenu : IUIScreen
    {
        IAimUI AimUI { get; }
        IDamageHitsUI DamageHits { get; }
        EvadeUI EvadeUI { get; }
        IShootAtTargetUI ShootAtTargetUI { get; }
        IBodySectionsUI EnemyBodySectionsUI { get; }
        FlashUI Flash { get; }
        CityDestroyUI CityDestroyUI { get; }
        TextByCharPrinter EventPrinter { get; }
        IBrokenWindowsUI BrokenWindowsUI { get; }
    }
}