using UnityEngine;

namespace GameCore.Core
{
    [CreateAssetMenu(menuName = "SO/" + nameof(BootSettings), fileName = nameof(BootSettings), order = 0)]
    public class BootSettings : ScriptableObject
    {
        public bool CapFPS;
        public int FpsCap = 60;
        public bool ShowFPSCanvas;
        [Space(10)]
        public bool InitAnalytics;
        public bool ShowPregameCheat;
        [Space(10)]
        public bool ClearAllSaves;
        [Space(10)] 
        public bool doPeriodicSave = true;
        public float dataSavePeriod = 10;
        [Space(10)] 
        public bool playMusicOnStart;
    }
}