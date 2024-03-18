#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Levels
{
    [CustomEditor(typeof(EditorLevelUtils))]
    public class EditorLevelUtilsEditor : Editor
    {
        private const float width = 110;
        private const float height = 40;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as EditorLevelUtils;
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (SwitchButton("<<"))
            {
                me.PrevInd();
            }
            if (SwitchButton(">>"))
            {
                me.NextInd();    
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (Button("SpawnTowers"))
            {
                me.E_SpawnAllTowers();
            }

            if (Button("Clear"))
            {
                me.E_ClearAll();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (Button("CameraToPlayer"))
            {
                me.E_SetCameraToPlayer();
            }
            if (Button("CameraToStart"))
            {
                me.E_SetCameraToStart();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(6);


            bool Button(string label)
            {
                return GUILayout.Button(label, 
                    GUILayout.Width(width),
                    GUILayout.Height(height));
            }
            
            bool SwitchButton(string label)
            {
                return GUILayout.Button(label, 
                    GUILayout.Width(35),
                    GUILayout.Height(35));
            }
        }
        
    }
}
#endif