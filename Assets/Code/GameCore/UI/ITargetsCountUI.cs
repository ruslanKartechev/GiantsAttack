namespace GameCore.UI
{
    public interface ITargetsCountUI
    {
        /// <summary>
        /// Also cashes "max" value
        /// </summary>
        void SetCount(int max, int current);
        void UpdateCount(int current);
        void Show(bool show);
    }
}