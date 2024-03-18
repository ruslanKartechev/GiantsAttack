using UnityEngine;
using UnityEngine.Serialization;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class BoatViewSettings : IBoatViewSettings
    {
        [SerializeField] private Material[] _color;
        [SerializeField] private Material[] _towerMaterials;
        
        
        public Material[] SideMaterial => _color;
        public Material[] TowerMaterials => _towerMaterials;
    }
}