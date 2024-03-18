using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class TowerSettings
    {
        public Material radiusMaterial;
        public Color uiColor;
        public List<TowerLevelSettings> levelSettings;

        public int MaxLevelInd => levelSettings.Count-1;
    }
}