namespace GiantsAttack
{
    public interface IHealth : IDamageable
    {
        float Health { get; }
        float MaxHealth { get; }
        float HealthPercent { get; }
        void SetMaxHealth(float val);
        void SetDamageable(bool canDamage);
        void ShowDisplay();
        void HideDisplay();
    }
}