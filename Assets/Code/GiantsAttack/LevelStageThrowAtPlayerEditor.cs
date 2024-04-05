#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(LevelStageThrowAtPlayer)), CanEditMultipleObjects()]
    public class LevelStageThrowAtPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as LevelStageThrowAtPlayer;
            const float width = 200;
            if (GUILayout.Button("Calc Move Time", GUILayout.Width(width)))
            {
                me.E_CalculateMoveTime();
            }
        }
    }
}
#endif