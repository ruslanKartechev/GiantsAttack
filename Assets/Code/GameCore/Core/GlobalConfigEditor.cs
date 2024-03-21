#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameCore.Core
{
    [CustomEditor(typeof(GlobalConfig))]
    public class GlobalConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as GlobalConfig;
            GUILayout.Space((10));
            if (GUILayout.Button($"Set static", GUILayout.Width(100)))
            {
                me.SetupStaticFields();
            }
        }
    }
}
#endif