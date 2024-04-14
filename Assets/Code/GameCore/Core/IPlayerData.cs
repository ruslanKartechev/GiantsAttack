
namespace GameCore.Core
{
    public interface IPlayerData
    {
        float Money{ get; set; }
        int LevelTotal { get; set; }
        bool SoundStatus { get; set; }
        float SoundVolume { get; set; }
        bool VibrationStatus { get; set; } 
    }
}