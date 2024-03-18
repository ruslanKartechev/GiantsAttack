using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class PerRendererMaterials
    {
        public List<Material> materials;

        public void Set(Renderer renderer)
        {
            renderer.sharedMaterials = materials.ToArray();
        }
    }
}