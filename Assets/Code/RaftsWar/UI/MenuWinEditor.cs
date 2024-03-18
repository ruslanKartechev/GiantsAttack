#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace RaftsWar.UI
{
    [CustomEditor(typeof(MenuWin))]
    public class MenuWinEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as MenuWin;
            GUILayout.Space(20);
            if (GUILayout.Button("Debug Show", GUILayout.Width(120)))
            {
                me.E_Show();   
            }
        }
    }
}
#endif