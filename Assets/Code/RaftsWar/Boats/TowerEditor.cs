#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(Tower))]
    public class TowerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as Tower;
            GUILayout.Space((10));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<<", GUILayout.Width(34)))
            {
                me.E_PrevLevel();
                Dirty();
            }
            if (GUILayout.Button(">>", GUILayout.Width(34)))
            {
                me.E_NextLevel();
                Dirty();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space((10));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Spawn", GUILayout.Width(90)))
            {
                me.E_SetLevel();
                Dirty();
            }
            if (GUILayout.Button("Zero", GUILayout.Width(90)))
            {
                me.E_ZeroLevel();
                Dirty();
            }
            GUILayout.EndHorizontal();

            void Dirty()
            {
                EditorUtility.SetDirty(me);
            }
        }
    }
}
#endif