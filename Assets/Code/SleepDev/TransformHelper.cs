using System;
using UnityEditor;
using UnityEngine;

namespace SleepDev
{
    public class TransformHelper : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private Transform _copyFrom;
        [SerializeField] private Transform _lookAt;
        public void LookAt()
        {
            if (_lookAt == null)
                return;
            transform.rotation = Quaternion.LookRotation(_lookAt.position - transform.position);
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        public void CopyPosRot()
        {
            if (_copyFrom == null)
                return;
            transform.SetPositionAndRotation(_copyFrom.position, _copyFrom.rotation);
        }
        
        public void ZeroLocalPosRot()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void OneScale()
        {
            transform.localScale = Vector3.one;
        }
#endif
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
            if (GUILayout.Button("Copy", GUILayout.Width(120)))
            {
                me.CopyPosRot();
                UnityEditor.EditorUtility.SetDirty(me);
            }
            if (GUILayout.Button("Look at", GUILayout.Width(120)))
            {
                me.LookAt();
                UnityEditor.EditorUtility.SetDirty(me);
            }
        }
    }
    #endif
}