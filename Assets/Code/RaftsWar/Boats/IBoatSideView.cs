namespace RaftsWar.Boats
{
    public interface IBoatSideView
    {
        /// <summary>
        /// Show side only, does not affect corners
        /// </summary>
        void ShowSide();
        /// <summary>
        /// Show both side and corners
        /// </summary>
        void ShowSideAndCorners();
        
        bool IsSideOn { get; }
        
        void ShowCorner1();
        void ShowCorner2();
        void HideCorners();
        void HideSide();
        void HideSideAndCorners();
        
        void Refresh();
        void SetView(IBoatViewSettings settings);
    }
}