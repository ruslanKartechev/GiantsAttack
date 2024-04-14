using GameCore.Core;
using SleepDev;

namespace GameCore.Levels
{
    public static class LevelUtils
    {
        public static void CallNextLevel()
        {
            GCon.LevelManager.NextLevel();
            GCon.LevelManager.LoadCurrent();
        }
        
        public static void CallReplay()
        {
            GCon.LevelManager.LoadCurrent();
        }
        
    }
}