using System.Collections.Generic;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class UnitViewSettings
    {
        public List<PerRendererMaterials> unitMaterials;
        public List<PerRendererMaterials> projectileMaterials;
    }
}