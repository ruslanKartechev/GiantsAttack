namespace GiantsAttack
{
    public class DummyHitCounter : IHitCounter
    {
        public int ShotsCount { get; set; }
        public int HitsCount { get; set; }
        public int MissCount { get; set; }
    }
}