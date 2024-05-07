#define HAS_SDK
using System.Collections.Generic;
using SleepDev;

namespace RaftsWar.Core
{
    public static class Analytics
    {
        public static void OnStarted(int index, string levelMode)
        {
#if HAS_SDK
#if UNITY_EDITOR
            CLog.Log($"[Analytics] Started event {index}, mode: {levelMode}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_start", new Dictionary<string, object>()
            {
                {"level_number", index+1},
                {"level_type", levelMode},
            });
            AppMetrica.Instance?.SendEventsBuffer();
#endif
        }

        public static void OnWin(int index, string levelMode, float playTime, int playerMisses, int playerHits)
        {
#if HAS_SDK
#if UNITY_EDITOR
            CLog.Log($"[Analytics] Finish event level {index+1}, playTime {playTime}, mode: {levelMode}, player missed {playerHits}");
 #endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"level_number", index + 1},
                {"level_type", levelMode},
                {"result", "win"},
                {"time", playTime},
                {"player_misses", playerMisses},
                {"player_hits", playerHits}
            });
#endif
        }
        
        public static void OnFailed(int index, string levelMode, string reason, float playTime, int playerMisses, int playerHits)
        {
#if HAS_SDK
#if UNITY_EDITOR
            CLog.Log($"[Analytics] Finish event level {index+1}, playTime {playTime}, mode: {levelMode}, player missed {playerHits}");
#endif
            MadPixelAnalytics.AnalyticsManager.CustomEvent("level_completed", new Dictionary<string, object>()
            {
                {"level_number", index+1},
                {"level_type", levelMode},
                {"result", "loose"},
                {"reason", reason},
                {"time", playTime},
                {"player_misses", playerMisses},
                {"player_hits", playerHits}
            });
#endif
        }
    }
}