#if UNITY_EDITOR
using UnityEditor;

namespace SleepDev.Utils
{
    [CustomEditor(typeof(EditorToPointRotator))]
    public class EditorToPointRotatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as EditorToPointRotator;
            me.SetRotation();
        }
    }
}
#endif