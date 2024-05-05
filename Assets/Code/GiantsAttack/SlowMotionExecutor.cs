using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class SlowMotionExecutor
    {
        [SerializeField] private ShooterSettings _slowMoShooterSettings;
        [SerializeField] private SlowMotionEffectSO _slowMotionEffect;
        private ShooterSettings _shooterSettingsBeforeChange;
        private IHelicopter _helicopter;
        
        public void BeginSlowMo()
        {
            _slowMotionEffect.Begin();
        }

        public void StopSlowMo()
        {
            _slowMotionEffect.Stop();
        }

        public void SetSettings(IHelicopter helicopter)
        {
            _helicopter = helicopter;
            _shooterSettingsBeforeChange = helicopter.Shooter.Settings;
            helicopter.Shooter.Settings = _slowMoShooterSettings;
        }

        public void RevertSettings()
        {
            _helicopter.Shooter.Settings = _shooterSettingsBeforeChange;
        }
    }
}