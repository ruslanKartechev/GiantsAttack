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
        
        void Init(IBodySectionsUI sectionsUI, float health);
        void Animate(string key, bool trigger);
        void Idle();
        void Roar();
        void Jump(bool transition);
        void KickUp();
        void Kill(bool chopped = false);
        void PreKillState();
        void SetMoveAnimationSpeed(float speed);
        void PickAndThrow(IThrowable target, Action onPickCallback, Action onThrowCallback, bool pickFromTop);
        void PunchStatic(string key, Action onHit, Action onCompleted);
        void Punch(string key, Action punchStartedCallback, Action onPunch, Action onAnimationEnd);
        void AlignPositionToAnimRootBone(bool playIdle);
        
    }
}