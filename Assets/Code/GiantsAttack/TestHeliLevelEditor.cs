#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(StageBasedLevel))]
    public class TestHeliLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as StageBasedLevel;
            GUILayout.Space((20));
            if (GUILayout.Button("E_Init", GUILayout.Width(100)))
            {
                me.E_Init();
            }
        }
    }
}
#endif