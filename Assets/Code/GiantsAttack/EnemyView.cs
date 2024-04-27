using UnityEngine;

namespace GiantsAttack
{
    [CreateAssetMenu(menuName = "SO/EnemyView", fileName = "EnemyView", order = 0)]
    public class EnemyView : ScriptableObject
    {
        public Material[] materials;

        public void SetView(SkinnedMeshRenderer renderer)
        {
            renderer.sharedMaterials = materials;
        }
        
        public void SetView(MeshRenderer renderer)
        {
            renderer.sharedMaterials = materials;
        }
    }
}