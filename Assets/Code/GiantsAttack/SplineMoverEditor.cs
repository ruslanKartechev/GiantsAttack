#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(SplineMover))]
    public class SplineMoverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(20);
            var me = target as SplineMover;
            const float width = 125f;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Set to Start", GUILayout.Width(width)))
                me.SetToStart();   
            if (GUILayout.Button($"Set to End", GUILayout.Width(width)))
                me.SetToEnd();   
            GUILayout.EndHorizontal();
            
            if (GUILayout.Button($"Add Point Here", GUILayout.Width(width)))
                me.E_AddTransformToCurrentPosition();
            
            me.E_AlignToInterpolateT();
        }
    }
}
#endif