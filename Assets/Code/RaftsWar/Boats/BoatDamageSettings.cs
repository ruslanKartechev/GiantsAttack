
namespace RaftsWar.Boats
{
    [System.Serializable]
    public class BoatDamageSettings
    {
        public int count;
        public float delay;
        public BoatViewSettingsSo damagedViewSo;
        
        public BoatViewSettings DamagedView => damagedViewSo.settings;

    }
}