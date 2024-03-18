#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Core
{
    [CustomEditor(typeof(LevelSpawner))]
    public class LevelSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as LevelSpawner;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<<", GUILayout.Width(50)))
            {
                me.E_Prev();
                Dirty();
            }
            if (GUILayout.Button(">>", GUILayout.Width(50)))
            {
                me.E_Next();
                Dirty();
            }
            if (GUILayout.Button("Clear", GUILayout.Width(80)))
            {
                me.E_Clear();
                Dirty();
            }
            GUILayout.EndHorizontal();

            void Dirty()
            {
                UnityEditor.EditorUtility.SetDirty(me);
            }
        }
    }
}
#endif