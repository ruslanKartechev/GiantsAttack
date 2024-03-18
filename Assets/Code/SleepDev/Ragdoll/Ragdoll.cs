using System.Collections.Generic;
using SleepDev.Utils;
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
        public int layerToSet;
        

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
                part.rb.gameObject.layer = layerToSet;
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
        public void GetAllParts()
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
                parts.Add(part);
            }
        }
        
        public void DestroyAll()
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
            }
            parts.Clear();
        }
        
        public void SetAllInterpolate()
        {
            foreach (var part in parts)
                part.rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
        
        public void SetAllExtrapolate()
        {
            foreach (var part in parts)
                part.rb.interpolation = RigidbodyInterpolation.Extrapolate;
        }

        public void SetAllNoInterpolate()
        {
            foreach (var part in parts)
            {
                part.rb.interpolation = RigidbodyInterpolation.None;
            }
        }
        
        public void SetProjection()
        {
            foreach (var part in parts)
            {
                var joint = part.rb.GetComponent<CharacterJoint>();
                if(joint == null)
                    continue;
                joint.enableProjection = true;
            }
        }

        public void SetNoProjection()
        {
            foreach (var part in parts)
            {
                var joint = part.rb.GetComponent<CharacterJoint>();
                if(joint == null)
                    continue;
                joint.enableProjection = false;
            }
        }
        
        public void SetAllPreprocess(bool preprocess)
        {
            foreach (var part in parts)
            {
                var joint = part.rb.gameObject.GetComponent<Joint>();
                if(joint != null)
                    joint.enablePreprocessing = preprocess;
            }
        }
#endif

      
    }
}