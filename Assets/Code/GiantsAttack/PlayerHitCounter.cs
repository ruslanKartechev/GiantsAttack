using SleepDev;

namespace GiantsAttack
{
    public class PlayerHitCounter : IHitCounter
    {
        public int ShotsCount { get; set; }
        public int HitsCount { get; set; }
        public int MissCount { get; set; }

        public void Log()
        {
            var msg = $"[HitCounter] ";
            msg += $"Shots count {ShotsCount}";
            msg += $"Hits count {HitsCount}";
            msg += $"Miss count {MissCount}";
            CLog.Log(msg);
        }
    }
}