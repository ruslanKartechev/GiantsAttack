using System;
using UnityEngine;

namespace RaftsWar.Boats
{
    public interface IDamageable
    {
        event Action<IDamageable> OnDied;
        bool IsDead { get; }
        bool IsAlive { get; }
        void TakeDamage(DamageArgs args);
        GameObject Go { get; }
        bool CanDamage { get; }

    }
}