#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(RaftAcceptArea))]
    public class RaftAcceptAreaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as RaftAcceptArea;
            var width = 90;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<<", GUILayout.Width(30)))
            {
                me.E_PrevInd();
            }            
            if (GUILayout.Button(">>", GUILayout.Width(30)))
            {
                me.E_NextInd();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Draw", GUILayout.Width(width)))
            {   
                me.E_DrawTest();
            }
            if (GUILayout.Button("Build", GUILayout.Width(width)))
            {   
                me.E_BuildAtIndex();
            }
            GUILayout.EndHorizontal();

        }
    }
}
#endif