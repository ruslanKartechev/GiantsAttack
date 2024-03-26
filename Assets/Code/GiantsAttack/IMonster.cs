using System;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public interface IMonster : ITarget
    {
        event Action<IMonster> OnKilled;
        
        IMonsterMover Mover { get; }
        IHealth Health { get; }
        IMonsterAnimEventReceiver AnimEventReceiver { get; }
        void Kill();
        
        void Init(IBodySectionsUI sectionsUI);
        
        void Idle();

        // Will rotate to and move to the target and play attack animation
        void Attack(Transform target);
        void Roar();
        void PickAndThrow(IThrowable target, Action onThrowCallback);
        
    }
}