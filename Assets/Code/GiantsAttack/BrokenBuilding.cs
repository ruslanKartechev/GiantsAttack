using System.Collections.Generic;
using GameCore.Cam;
using SleepDev.Utils;
using UnityEditor;
using UnityEngine;

namespace GiantsAttack
{
    public class BrokenBuilding : MonoBehaviour,IBrokenBuilding
    {
        [SerializeField] private bool _shakeCameraOnHit = true;
        [SerializeField] private float _force;
        [SerializeField] private List<Part> _parts;
        [SerializeField] private List<MeshRenderer> _rendsToDisable;
        [SerializeField] private ParticleSystem _particles;

        public void Break()
        {
            foreach (var rend in _rendsToDisable)
                rend.enabled = false;
            
            foreach (var p in _parts)
                p.Activate();
            foreach (var p in _parts)
                p.Push(_force);
            
            _particles.gameObject.SetActive(true);
            _particles.Play();
            if (_shakeCameraOnHit)
                CameraContainer.Shaker.PlayDefault();
        }
        
#if UNITY_EDITOR
        [ContextMenu("E_GetOrBuild")]
        public void E_GetOrBuild()
        {
            var count = transform.childCount;
            _parts = new List<Part>(count);
            for (var i = 0; i < count; i++)
            {
                var ch = transform.GetChild(i);
                if (ch.gameObject.TryGetComponent<ParticleSystem>(out var ps))
                {
                    continue;
                }
                var rb = ch.gameObject.GetOrAdd<Rigidbody>();
                rb.isKinematic = true;
                var cl = ch.GetComponent<Collider>();
                if (cl != null)
                    DestroyImmediate(cl);
                var coll = ch.gameObject.GetOrAdd<MeshCollider>();
                coll.enabled = false;
                coll.convex = true;
                var part = new Part()
                {
                    rb = rb,
                    collider = coll
                };
                _parts.Add(part);
                UnityEditor.EditorUtility.SetDirty(ch.gameObject);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
        
        
        [System.Serializable]
        public class Part
        {
            public Rigidbody rb;
            public Collider collider;

            public void Push(float force)
            {
                rb.AddForce(rb.transform.localPosition.normalized * force);
            }
            
            public void Activate()
            {
                rb.gameObject.SetActive(true);
                rb.isKinematic = false;
                collider.enabled = true;
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(BrokenBuilding))]
    public class BrokenBuildingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as BrokenBuilding;
            GUILayout.Space(25);
            if (GUILayout.Button("Get Or Build", GUILayout.Width(120)))
                me.E_GetOrBuild();                
        }
    }
    #endif
}