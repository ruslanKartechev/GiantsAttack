#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(CameraRailMover))]
    public class CameraRailMoverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as CameraRailMover;
            if (GUILayout.Button("Look points", GUILayout.Width(100)))
            {
                me.LookAt();
            }
        }
    }
}
#endif