namespace RaftsWar.Core
{
    public static class GlobalState
    {
        public static bool NoBootSceneMode { get; set; } = true;
        public static bool DevSceneMode { get; set; } = true;
        public static bool SingleModeInitiated { get; set; } = false;

    }
}