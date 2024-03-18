#if UNITY_EDITOR
using UnityEditor;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(TowerBlocksBuilder)), CanEditMultipleObjects]
    public class TowerBlocksBuilderEditor : Editor
    {
        
    }
}
#endif