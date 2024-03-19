namespace GiantsAttack
{
    public interface IHitCounter
    {
        int ShotsCount { get; set; }
        int HitsCount { get; set; }
        int MissCount { get; set; }
        
    }
}