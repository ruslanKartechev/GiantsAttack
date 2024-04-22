#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GameCore.UI
{
    [CustomEditor(typeof(FlashUI))]
    public class FlashUIEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as FlashUI;
            if (GUILayout.Button("Play", GUILayout.Width(100)))
            {
                me.Play();
            } 
        }
    }
}
#endif