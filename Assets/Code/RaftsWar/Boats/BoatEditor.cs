#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(Boat))]
    public class BoatEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as Boat;
            GUILayout.Space(10);
            if (GUILayout.Button("Log Str", GUILayout.Width(120)))
            {
                me.E_LogStructure();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Break last", GUILayout.Width(120)))
            {
                me.E_BreakOffLast();
            }
        }
    }
}
#endif