#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(Placer))]
    public class PlacerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as Placer;
            GUILayout.Space(20);
            if (GUILayout.Button($"Default Point", GUILayout.Width(120)))
            {
                me.SetDefault();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"<<", GUILayout.Width(50)))
            {
                me.Prev();
            }
            if (GUILayout.Button($">>",GUILayout.Width(50)))
            {
                me.Next();
            }
            GUILayout.Space((10));
            if (GUILayout.Button($"P",GUILayout.Width(50)))
            {
                me.Place();
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button($"On Off",GUILayout.Width(50)))
            {
                me.OnOff();
            }
        }
    }
}
#endif