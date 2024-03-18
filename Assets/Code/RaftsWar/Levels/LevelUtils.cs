using System.Collections.Generic;
using RaftsWar.UI;
using SleepDev;

namespace RaftsWar.Levels
{
    public static class LevelUtils
    {
        public static void SetupNames(LevelTeamsManager teamsManager, IGameplayMenu gameplayMenu)
        {
            // CLog.Log($"[LevelUtils] Setup names");
            var nm = gameplayMenu.NamesUIManager;
            nm.AddPlayer(teamsManager.PlayerBoat);
            foreach (var team in teamsManager.EnemyTeams)
                nm.AddPlayer(team.EnemyBoat);
        }

        public static void CallWinScreen(float addedTowerProgress)
        {
            CLog.Log($"[LevelUtils] Call win screen");
            var ui = GCon.UIFactory.GetCompletedMenu() as IMenuWin;
            ui.Show(addedTowerProgress, CallNextLevel, () => {});
            HapticPlayer.HapticWin();
        }

        public static void CallFailScreen()
        {
            CLog.Log($"[LevelUtils] Call win screen");
            var ui = GCon.UIFactory.GetFailedMenu() as IMenuFail;
            ui.Show(CallReplay, () => {});
        }
        
        public static void CallNextLevel()
        {
            CLog.Log($"[LevelUtils] Call next level");
            GCon.DataSaver.Save();
            GCon.PoolsManager.RecollectAll();
            GCon.LevelManager.NextLevel();
            GCon.LevelManager.LoadCurrent();
        }

        public static void CallReplay()
        {
            CLog.Log($"[LevelUtils] Call replay level");
            GCon.PoolsManager.RecollectAll();
            GCon.LevelManager.LoadCurrent();
        }

        public static void SendEventLevelStart()
        {
            try
            {
                MadPixelAnalytics.AnalyticsManager.CustomEvent("level_started", new Dictionary<string, object>()
                {
                    {"level_number", GCon.PlayerData.LevelTotal}
                });
            }
            catch (System.Exception ex)
            {
                CLog.Log($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        public static void SendEventLevelWin()
        {
            try
            {
                MadPixelAnalytics.AnalyticsManager.CustomEvent("level_finish", new Dictionary<string, object>()
                {
                    {"level_number", GCon.PlayerData.LevelTotal},
                    {"result", "win"},
                });
            }
            catch (System.Exception ex)
            {
                CLog.Log($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void SendEventLevelFail()
        {
            try
            {
                MadPixelAnalytics.AnalyticsManager.CustomEvent("level_finish", new Dictionary<string, object>()
                {
                    {"level_number", GCon.PlayerData.LevelTotal},
                    {"result", "fail"},
                });
            }
            catch (System.Exception ex)
            {
                CLog.Log($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
    
        }


    }
}