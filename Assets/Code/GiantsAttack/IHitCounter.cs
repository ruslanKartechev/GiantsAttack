namespace GiantsAttack
{
    public interface IHitCounter
    {
        int ShotsCount { get; set; }
        int HitsCount { get; set; }
        int HeadShotsCount { get; set; }
        int MissCount { get; set; }
        int BestStreak { get; set; }

    }
}