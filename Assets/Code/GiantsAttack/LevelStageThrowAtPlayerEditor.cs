#if UNITY_EDITOR
using UnityEditor;

namespace GiantsAttack
{
    [CustomEditor(typeof(LevelStageThrowAtPlayer))]
    public class LevelStageThrowAtPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as LevelStageThrowAtPlayer;
            const float width = 200;
            // if (GUILayout.Button("Boss to point", GUILayout.Width(width)))
            // {
            //     
            // }
            // if (GUILayout.Button("Player look at", GUILayout.Width(width)))
            // {
            //     
            // }
            // if (GUILayout.Button("", GUILayout.Width(width)))
            // {
            //     
            // }

        }
    }
}
#endif