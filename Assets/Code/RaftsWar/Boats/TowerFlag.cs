using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerFlag : MonoBehaviour
    {
        [SerializeField] private bool _canWork = true;
        [SerializeField] private List<MeshRenderer> _renderers;
        
        public void Show()
        {
            if (!_canWork)
                return;
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetView(Team team)
        {
            var mat = team.UnitsView.unitMaterials[0].materials[0];
            foreach (var rend in _renderers)
                rend.sharedMaterial = mat;
        }
        
    }
}