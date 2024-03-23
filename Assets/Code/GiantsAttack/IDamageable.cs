using System;

namespace GiantsAttack
{
    public interface IDamageable
    {
        event Action<IDamageable> OnDead;
        event Action<IDamageable> OnDamaged;
        bool CanDamage { get; }

        void TakeDamage(DamageArgs args);
    }
}