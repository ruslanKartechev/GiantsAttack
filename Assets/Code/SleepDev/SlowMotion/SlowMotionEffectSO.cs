using UnityEngine;

namespace SleepDev.SlowMotion
{
    [CreateAssetMenu(menuName = "SO/" + nameof(SlowMotionEffectSO), fileName = nameof(SlowMotionEffectSO), order = 0)]
    public class SlowMotionEffectSO : ScriptableObject
    {
        [SerializeField] private SlowMotionEffect _effect;
        public SlowMotionEffect Effect => _effect;

        public void Begin()
        {
            GameCore.Core.GCon.SlowMotion.Begin(Effect);
        }

        public void Stop()
        {
            GameCore.Core.GCon.SlowMotion.Exit(Effect);
        }
    }
}