#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameCore.UI
{
    [CustomEditor(typeof(RouletteUI))]
    public class RouletteUIEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(30);
            var me = target as RouletteUI;
            
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<<", GUILayout.Width(45)))
            {
                me.E_Prev();
            }
            if (GUILayout.Button(">>", GUILayout.Width(45)))
            {
                me.E_Next();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Start", GUILayout.Width(100)))
            {
                me.E_Start();
            }

        }
    }
}
#endif