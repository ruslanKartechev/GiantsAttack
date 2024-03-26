#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(Helicopter))]
    public class HelicopterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var me = target as Helicopter;
            base.OnInspectorGUI();
            if (GUILayout.Button("Kill", GUILayout.Width(100)))
            {
                me.Kill();
            }
        }
    }
}
#endif