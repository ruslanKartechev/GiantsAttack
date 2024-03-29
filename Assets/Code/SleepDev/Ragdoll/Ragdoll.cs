using System.Collections.Generic;
using SleepDev.Utils;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SleepDev.Ragdoll
{
    public partial class Ragdoll : IRagdoll
    {
        public List<RagdollPart> parts;
        public List<Transform> ignoredParents;
        [Header("Editor")]
        [SerializeField] private int e_layerToSet;
        [SerializeField] private float e_massToSet;
        public int E_layerToSet => e_layerToSet;
        
        public override bool IsActive { get; protected set; }

        public override void Activate()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                foreach (var part in parts)
                    part.On();
                return;
            }
            #endif
            if (IsActive)
                return;
            IsActive = true;
            foreach (var part in parts)
                part.On();
        }

        public override void Deactivate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                foreach (var part in parts)
                    part.Off();
                return;
            }
#endif
            if (!IsActive)
                return;
            IsActive = false;
            foreach (var part in parts)
                part.Off();
        }

        public override void ActivateAndPush(Vector3 force)
        {
            var pushable = new List<RagdollPart>();
            IsActive = true;
            foreach (var part in parts)
            {
                part.On();
                // part.rb.velocity = Vector3.zero;
                if(part.push)
                    pushable.Add(part);
            }
            foreach (var part in pushable)
            {
                part.Push(force);
            }
        }
        
        public void SetLayer()
        {
            foreach (var part in parts)
                part.rb.gameObject.layer = e_layerToSet;
        }

        public void SetCollidersOnly()
        {
            foreach (var part in parts)
            {
                part.collider.enabled = true;
                part.collider.isTrigger = false;
                part.rb.isKinematic = true;
            }
        }
        
        public override void ZeroVelocities()
        {
            foreach (var part in parts)
            {
                part.rb.velocity = Vector3.zero;
            }   
        }
        
        public override void SetGravityAll(bool on)
        {
            foreach (var part in parts)
            {
                part.rb.useGravity = on;
            }   
        }
        
#if UNITY_EDITOR
        public void E_GetParts()
        {
            ignoredParents.RemoveAll(t => t == null);
            var gos = GameUtils.GetFromAllChildren<Transform>(transform, (tr) =>
            {
                foreach (var parent in ignoredParents)
                {
                    if (tr == parent)
                        return false;
                    if (GameUtils.IsChildOf(tr, parent))
                        return false;
                }
                var coll = tr.GetComponent<Collider>();
                var rb = tr.GetComponent<Rigidbody>();
                if (rb != null && coll != null)
                    return true;
                return false;
            });
            parts = new List<RagdollPart>(gos.Count);
            foreach (var go in gos)
            {
                var part = new RagdollPart()
                {
                    rb = go.GetComponent<Rigidbody>(),
                    collider =  go.GetComponent<Collider>(),
                    name = go.name
                };
                UnityEditor.EditorUtility.SetDirty(go);
                parts.Add(part);
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        public void E_DestroyAll()
        {
            foreach (var pp in parts)
            {
                var go = pp.rb.gameObject;
                var joint = go.GetComponent<Joint>();
                if(joint != null)
                    DestroyImmediate(joint);
                if(pp.rb != null)
                    DestroyImmediate(pp.rb);
                if(pp.collider != null)
                    DestroyImmediate(pp.collider);
                UnityEditor.EditorUtility.SetDirty(pp.rb.gameObject);
            }
            parts.Clear();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        public void E_SetInterpolate()
        {
            foreach (var part in parts)
            {
                if(part.rb == null)
                    continue;
                part.rb.interpolation = RigidbodyInterpolation.Interpolate;
                UnityEditor.EditorUtility.SetDirty(part.rb);
            }
        }
        
        public void E_SetExtrapolate()
        {
            foreach (var part in parts)
            {
                if(part.rb == null)
                    continue;
                part.rb.interpolation = RigidbodyInterpolation.Extrapolate;
                UnityEditor.EditorUtility.SetDirty(part.rb);
            }
        }

        public void E_SetNoInterpolate()
        {
            foreach (var part in parts)
            {
                if(part.rb == null)
                    continue;
                part.rb.interpolation = RigidbodyInterpolation.None;
                UnityEditor.EditorUtility.SetDirty(part.rb);
            }
        }
        
        public void E_SetProjection()
        {
            foreach (var part in parts)
            {
                var joint = part.rb.GetComponent<CharacterJoint>();
                if(joint == null)
                    continue;
                joint.enableProjection = true;
                UnityEditor.EditorUtility.SetDirty(joint);
            }
        }

        public void E_SetNoProjection()
        {
            foreach (var part in parts)
            {
                var joint = part.rb.GetComponent<CharacterJoint>();
                if(joint == null)
                    continue;
                joint.enableProjection = false;
                UnityEditor.EditorUtility.SetDirty(joint);
            }
        }
        
        public void E_SetPreprocessAll(bool preprocess)
        {
            foreach (var part in parts)
            {
                var joint = part.rb.gameObject.GetComponent<Joint>();
                if(joint != null)
                    joint.enablePreprocessing = preprocess;
                UnityEditor.EditorUtility.SetDirty(joint);
            }
        }

        public void E_SetMassAll()
        {
            foreach (var part in parts)
            {
                if(part.rb == null)
                    continue;
                part.rb.mass = e_massToSet;
                UnityEditor.EditorUtility.SetDirty(part.rb);
            }
        }
#endif

      
    }
}