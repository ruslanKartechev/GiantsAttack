#if UNITY_EDITOR
using SleepDev.Utils.EditorUtils;
using UnityEditor;
using UnityEngine;

namespace SleepDev.UIUtils
{
    [CustomEditor(typeof(LayoutSwitcher))]
    public class LayoutSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as LayoutSwitcher;
            GUILayout.BeginHorizontal();
            if (EU.ButtonBig("Prev", Color.cyan))
                me.SetPrevLayout();
            if (EU.ButtonBig("Next", Color.cyan))
                me.SetNextLayout();   
            GUILayout.EndHorizontal();
        }
    }
}
#endif