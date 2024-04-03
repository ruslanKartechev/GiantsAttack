#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(TestHeliLevel))]
    public class TestHeliLevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as TestHeliLevel;
            GUILayout.Space((20));
            if (GUILayout.Button("E_Init", GUILayout.Width(100)))
            {
                me.E_Init();
            }
        }
    }
}
#endif