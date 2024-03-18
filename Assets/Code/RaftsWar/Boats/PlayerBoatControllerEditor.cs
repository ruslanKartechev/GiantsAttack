#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    [CustomEditor(typeof(BoatPlayer))]
    public class PlayerBoatControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BoatPlayer;
            const float size = 150;
            if (GUILayout.Button($"Log state", GUILayout.Width(size)))
            {
                me.E_DebugState();
            }
            if (GUILayout.Button($"Kill", GUILayout.Width(size)))
            {
                me.Kill();
            }
            if (GUILayout.Button($"Push out", GUILayout.Width(size)))
            {
                me.E_Push();
            }
            if (GUILayout.Button($"Collide push", GUILayout.Width(size)))
            {
                me.E_CollisionPushback();
            }
        }
    }
}
#endif