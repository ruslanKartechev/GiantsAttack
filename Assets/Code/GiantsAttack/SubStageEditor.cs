#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(SubStage))]
    public class SubStageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as SubStage;
            GUILayout.Space(10);
            if (GUILayout.Button("Get Listeners", GUILayout.Width(120)))
            {
                me.E_GetListeners();
            }
        }
    }
}
#endif