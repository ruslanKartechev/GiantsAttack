using System;
using GiantsAttack;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace SleepDev.Splines
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(SplineManager))]
    public class SplineManagerEditor : Editor
    {
        private void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            var normalWidth = 100;
            var me = target as SplineManager;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add example", GUILayout.Width(normalWidth)))
            {
                me.AddSegment();
            }
            if (GUILayout.Button("Clear", GUILayout.Width(normalWidth)))
            {
                me.Clear();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            base.OnInspectorGUI();
        }

        public void OnSceneGUI()
        {
            
        }
    }
    #endif
    
    
    public class SplineManager : MonoBehaviour
    {
        
        [SerializeField] private int _sampleCount = 10;
        [SerializeField] private Spline _spline;
        public Spline spline => _spline;
        
#if UNITY_EDITOR
        [Space(10)] [Header("Editor GUI")] 
        public bool e_doDraw;
        public Color e_color = Color.blue;
        public float e_pointSize = 1f;
        
        
        private void OnDrawGizmos()
        {
            if(e_doDraw)
                DrawSpline();
        }

        public void DrawSpline()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = e_color;
            var splineSubSplines = _spline.SubSplines;
            if (splineSubSplines.Count == 0)
                return;
            for (var i = 0; i < splineSubSplines.Count; i++)
            {
                var segment = splineSubSplines[i];
                Gizmos.DrawSphere(segment.p0.worldPosition, e_pointSize);
                Gizmos.DrawSphere(segment.p3.worldPosition, e_pointSize);
                var t = 0f;
                var prevPoint = Spline.GetCubicBezier(segment, t);
                for (var ti = 1f; ti <= _sampleCount; ti++)
                {
                    t = ti / _sampleCount;
                    var currPoint = Spline.GetCubicBezier(segment, t);
                    Gizmos.DrawLine(prevPoint, currPoint);
                    prevPoint = currPoint;
                }
            }
            
            Gizmos.color = oldColor;
        }

        public void AddSegment()
        {
            const float p4Length = 10;
            var point = new GameObject("temp").transform;
            if (_spline.SubSplines.Count > 0)
            {
                point.position = _spline.SubSplines[^1].p3.worldPosition;
                point.rotation = _spline.SubSplines[^1].p3.worldRotation;
            }
            else
            {
                point.SetPositionAndRotation(transform.position, transform.rotation);   
            }
            var p1 = new SplineNode(point.position, point.rotation);
            var p4 = new SplineNode(point.position + Vector3.forward * p4Length, point.rotation);
            var subSpline = new SubSpline3(p1,p4);
            _spline.SubSplines.Add(subSpline);
            DestroyImmediate(point.gameObject);
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void Clear()
        {
            _spline.SubSplines.Clear();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        
        
    }
}