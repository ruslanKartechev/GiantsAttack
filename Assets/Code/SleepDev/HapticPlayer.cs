#define HAPTIC
#if HAPTIC
using MoreMountains.NiceVibrations;

namespace SleepDev
{
    public class HapticPlayer
    {
        public static void Init()
        {
        }

        public static void HapticMin()
        {
            // CLog.Log($"[Haptic] min");
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
        }
        
        public static void HapticStrong()
        {
            // CLog.Log($"[Haptic] strong");
            MMVibrationManager.Haptic(HapticTypes.Warning);
        }
        
        public static void HapticWin()
        {
            // CLog.Log($"[Haptic] win");
            MMVibrationManager.Haptic(HapticTypes.Success);
        }
        

    }
}
#endif