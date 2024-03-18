#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(TowerSignaller1))]
    public class TowerSignaller1Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as TowerSignaller1;
            const float width = 90;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stage 1", GUILayout.Width(width)))
            {
                me.Tower1();
            }
            if (GUILayout.Button("Stage 2", GUILayout.Width(width)))
            {
                me.Tower2();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Stage 3", GUILayout.Width(width)))
            {
                me.Tower3();
            }
            if (GUILayout.Button("Stage 4", GUILayout.Width(width)))
            {
                me.Tower4();
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif