using GameCore.Core;
using GameCore.UI;

namespace GiantsAttack
{
    public static class DamageCalculator
    {
        public static void AddDamage(ITarget target, DamageArgs args)
        {
            if (target.Damageable.CanDamage == false)
                return;
            var dtype = DamageIndicationType.Default;
            if (target.Damageable is BodySection section)
            {
                if (section.IsHead)
                {
                    dtype = DamageIndicationType.Headshot;
                    args.damage *= GlobalConfig.HeadshotDamageMultiplier;
                }
            }
            if (dtype != DamageIndicationType.Headshot && args.isCrit)
                dtype = DamageIndicationType.Critical;
            var ui = GCon.UIFactory.GetGameplayMenu() as IGameplayMenu;
            ui.DamageHits.ShowHit(args.point, args.damage, dtype);
            target.Damageable.TakeDamage(args);
        }
    }
}