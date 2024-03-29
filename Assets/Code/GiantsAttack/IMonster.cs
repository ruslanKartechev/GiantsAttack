using System;
using GameCore.UI;
using UnityEngine;
using System.Collections.Generic;


namespace GiantsAttack
{
    public interface IMonster : ITarget
    {
        event Action<IMonster> OnDefeated;
        
        IMonsterMover Mover { get; }
        IHealth Health { get; }
        IMonsterAnimEventReceiver AnimEventReceiver { get; }
        BodySectionsManager BodySectionsManager { get; }
        public Transform Point { get; }
        public Transform LookAtPoint { get; }
        public List<Transform> DamagePoints { get; }
        
        void Init(IBodySectionsUI sectionsUI);
        void Idle();
        void Kill();
        void PreKillState();
        void Roar();
        void PickAndThrow(IThrowable target, Action onThrowCallback);
        
    }
}