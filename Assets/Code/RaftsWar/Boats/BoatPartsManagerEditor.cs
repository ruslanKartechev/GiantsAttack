#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(BoatPartsManager))]
    public class BoatPartsManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BoatPartsManager;
            var width = 140;
            GUILayout.Space(10);
            if (GUILayout.Button("GetSpawnPoints", GUILayout.Width(width)))
            {
                me.E_GetSpawnPoints();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("HighlightInitialPoints", GUILayout.Width(width)))
            {
                me.E_HighlightInitialPoints();
            }
            GUILayout.Space(10);

        }
    }
}
#endif