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

    public interface IHealth : IDamageable
    {
        float Health { get; }
        float MaxHealth { get; }
        float HealthPercent { get; }
        void SetMaxHealth(float val);
        void SetDamageable(bool canDamage);
    }
}