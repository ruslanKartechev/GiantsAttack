using System;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public interface IMonster : ITarget
    {
        event Action<IMonster> OnDefeated;
        
        IMonsterMover Mover { get; }
        IHealth Health { get; }
        IMonsterAnimEventReceiver AnimEventReceiver { get; }
        BodySectionsManager BodySectionsManager { get; }
        Transform Point { get; }
        
        void Init(IBodySectionsUI sectionsUI);
        void Idle();
        void Kill();
        void PreKillState();
        void Roar();
        void PickAndThrow(IThrowable target, Action onThrowCallback);
        
    }
}