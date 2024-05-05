#define VIBRATION_ON
using MoreMountains.NiceVibrations;

namespace SleepDev.Vibration
{
    public class VibrationManager
    {
        public static VibrationManager VibrManager { get; private set; }
        
        private bool _isOn;
        
        public VibrationManager(bool isOn)
        {
            if (VibrManager != null)
            {
                CLog.LogRed($"Vibration manager already exists");
                return;
            }
            _isOn = isOn;
            VibrManager = this;
        }

        public void PlaySimple()
        {
            #if VIBRATION_ON
            if (!_isOn)
                return;
            MMVibrationManager.Vibrate();
            #endif
        }

        public void SetStatus(bool isOn)
        {
            _isOn = isOn;
            CLog.LogWhite($"[VibrationManager] status set {isOn}");
        }

        public bool IsOn => _isOn;
    }
}