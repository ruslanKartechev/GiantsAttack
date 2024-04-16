using UnityEngine;

namespace GameCore.UI
{
    public interface IGameplayMenu : IUIScreen
    {
        IAimUI AimUI { get; }
        IDamageHitsUI DamageHits { get; }
        EvadeUI EvadeUI { get; }
        IShootAtTargetUI ShootAtTargetUI { get; }
        IBodySectionsUI EnemyBodySectionsUI { get; }
        void AddBodySectionsUI(GameObject prefab);
    }
}