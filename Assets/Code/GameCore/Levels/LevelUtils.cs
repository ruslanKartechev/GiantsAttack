using GameCore.Core;

namespace GameCore
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