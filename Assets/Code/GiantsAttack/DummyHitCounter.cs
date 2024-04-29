namespace GiantsAttack
{
    public class DummyHitCounter : IHitCounter
    {
        public int ShotsCount { get; set; }
        public int HitsCount { get; set; }
        public int HeadShotsCount { get; set; }
        public int MissCount { get; set; }
        public int BestStreak { get; set; }
    }
}