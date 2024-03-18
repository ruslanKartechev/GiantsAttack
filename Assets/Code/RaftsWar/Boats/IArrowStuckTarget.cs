namespace RaftsWar.Boats
{
    public interface IArrowStuckTarget
    {
        void StuckArrow(IArrow arrow);
        void HideAll();
    }
}