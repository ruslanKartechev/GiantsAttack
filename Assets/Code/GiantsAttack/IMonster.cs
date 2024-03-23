using System;
using UnityEngine;

namespace GiantsAttack
{
    public interface IMonster : ITarget
    {
        event Action<IMonster> OnKilled;
        
        IMonsterMover Mover { get; }
        IHealth Health { get; }
        
        void Kill();
        
        void Init();
        
        void Idle();

        // Will rotate to and move to the target and play attack animation
        void Attack(Transform target);
        void Roar();
    }
}