
namespace RaftsWar.Core
{
    public interface IPlayerData
    {
        /// <summary>
        /// Notifies on money count updated. (OldCount, NewCount)
        /// </summary>
        float Money{ get; set; }
        int LevelTotal { get; set; }
        int TowerLevel { get; set; }
        float TowerProgress { get; set; }
    }
}