#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(BoatPartsSpawner))]
    public class BoatPartsSpawnerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BoatPartsSpawner;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Gizmo On", GUILayout.Width(75)))
            {
                me.GizmoOn();    
            }
            if (GUILayout.Button("Gizmo On", GUILayout.Width(75)))
            {
                me.GizmoOff();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            if (GUILayout.Button("GetPoints", GUILayout.Width(100)))
            {
                me.E_GetSpawnPoints();
            }
            if (GUILayout.Button("SetColors", GUILayout.Width(100)))
            {
                me.E_SetEditorPointsColors();
            }

        }
    }
}

#endif