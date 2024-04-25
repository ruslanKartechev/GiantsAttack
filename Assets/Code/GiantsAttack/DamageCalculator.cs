using GameCore.Core;
using GameCore.UI;

namespace GiantsAttack
{
    public static class DamageCalculator
    {
        public static float CritDamage = 70;
        public static float[] CritChanges = new[] { .1f,.2f,.3f };
        public static float[] DamageMults = new[] { 1f, 1.1f, 1.2f };
        private static BodySection _prevSection;
        private static int _streakCount = 0;
        
        public static void AddDamage(ITarget target, DamageArgs args)
        {
            if (target.Damageable.CanDamage == false)
                return;
            var ui = (GCon.UIFactory.GetGameplayMenu() as IGameplayMenu).DamageHits;

            var dtype = DamageIndicationType.Default;
            if (target.Damageable is BodySection section)
            {
                if (section.IsHead)
                {
                    dtype = DamageIndicationType.Headshot;
                    args.damage *= GlobalConfig.HeadshotDamageMultiplier;
                }
                else
                {
                    var level = section.HealthLevel;
                    args.damage *= DamageMults[level];
                    var chance = UnityEngine.Random.Range(0f, 1f);
                    if (chance < CritChanges[level])
                    {
                        args.damage = CritDamage;
                        dtype = DamageIndicationType.Critical;
                    }
                }

                if (_prevSection == section)
                {
                    _streakCount++;
                    if (_streakCount >= 2)
                        ui.ShowSteak(_streakCount);
                }
                else
                {
                    _prevSection = null;
                    _streakCount = 0;
                    ui.HideStreak();  
                }
                _prevSection = section;
            }
            ui.ShowHit(args.point, args.damage, dtype);
            target.Damageable.TakeDamage(args);
        }
    }
}