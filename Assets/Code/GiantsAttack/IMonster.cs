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
        void Roar();
        void Kill();
        void PreKillState();
        void PickAndThrow(IThrowable target, Action onPickCallback, Action onThrowCallback);
        void Punch(string key, Action punchStartedCallback, Action onPunch);

    }
}