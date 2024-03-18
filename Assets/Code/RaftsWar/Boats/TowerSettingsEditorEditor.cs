#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(TowerSettingsEditor))]
    public class TowerSettingsEditorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            const int width = 140;
            base.OnInspectorGUI();
            GUILayout.Space((15));
            var me = target as TowerSettingsEditor;
            if (GUILayout.Button("Set All", GUILayout.Width(width)))
            {
                me.SetAll();
            }
            if (GUILayout.Button("Set Radius", GUILayout.Width(width)))
            {
                me.SetRadiusAll();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Set Fire rate", GUILayout.Width(width)))
            {
                me.SetFireRateAll();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Set Damage", GUILayout.Width(width)))
            {
                me.SetDamageAll();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("SetTowerHealth", GUILayout.Width(width)))
            {
                me.SetTowerHealthAll();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("SetUpgradePoints", GUILayout.Width(width)))
            {
                me.SetUpgradePointsAll();
            }
            GUILayout.Space((10));

        }
    }
}

#endif