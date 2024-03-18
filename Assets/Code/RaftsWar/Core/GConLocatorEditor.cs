#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Core
{
    [CustomEditor(typeof(GConLocatorSo))]
    public class GConLocatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as GConLocatorSo;
            if (GUILayout.Button("Release", GUILayout.Width(100)))
                me.ReleaseMode();
            if (GUILayout.Button("Debug", GUILayout.Width(100)))
                me.DebugMode();
        }
    }
}
#endif