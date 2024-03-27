#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    [CustomEditor(typeof(MonsterController))]
    public class MonsterControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(20);
            var me = target as MonsterController;
            if (GUILayout.Button($"Kill", GUILayout.Width(100)))
            {
                me.Kill();
            }
        }
    }
}
#endif