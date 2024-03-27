using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    public class TransformHelper : MonoBehaviour
    {
        public void ZeroLocalPosRot()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void OneScale()
        {
            transform.localScale = Vector3.one;
        }
        
    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(TransformHelper))]
    public class TransformHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as TransformHelper;
            GUILayout.Space(20);
            if (GUILayout.Button("Zero Pos Rot", GUILayout.Width(120)))
            {
                me.ZeroLocalPosRot();
                UnityEditor.EditorUtility.SetDirty(me);
            }
            if (GUILayout.Button("One Scale", GUILayout.Width(120)))
            {
                me.OneScale();
                UnityEditor.EditorUtility.SetDirty(me);
            }

        }
    }
    #endif
}