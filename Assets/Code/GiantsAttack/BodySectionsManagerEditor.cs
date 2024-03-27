#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(BodySectionsManager))]
    public class BodySectionsManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BodySectionsManager;
            const float width = 120;
            GUILayout.Space(20);
            if (GUILayout.Button("Set health", GUILayout.Width(width)))
            {
                me.E_SetHealthAll();
            }
            if (GUILayout.Button("Get flickers", GUILayout.Width(width)))
            {
                me.E_GetFlickers();
            }
            if (GUILayout.Button("Add Targets", GUILayout.Width(width)))
            {
                me.E_AddOrGrabBodyTargets();
            }
            if (GUILayout.Button("Assign to Sections", GUILayout.Width(width)))
            {
                me.E_AssignToSections();
            }
        }
    }
}

#endif