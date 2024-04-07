#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(PlayerMover))]
    public class PlayerMoverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(20);
            var me = target as PlayerMover;
            const float width = 125f;
            if (GUILayout.Button($"Calculate time", GUILayout.Width(width)))
                me.E_CalculateTimeFromSpeed();
        }
    }
}
#endif