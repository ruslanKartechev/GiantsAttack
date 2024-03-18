#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(BoatPart))]
    public class BoatPartEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BoatPart;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Top", GUILayout.Width(90)))
            {
                me.E_TopSideViewOnOff();
            }
            if (GUILayout.Button("Bot", GUILayout.Width(90)))
            {
                me.E_BotSideViewOnOff();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Right", GUILayout.Width(90)))
            {
                me.E_RightSideViewOnOff();
            }
            if (GUILayout.Button("Left", GUILayout.Width(90)))
            {
                me.E_LeftSideViewOnOff();
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif