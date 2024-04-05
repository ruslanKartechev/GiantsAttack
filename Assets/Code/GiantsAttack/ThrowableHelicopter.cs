using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class ThrowableHelicopter : SimpleThrowable
    {
        [SerializeField] private List<ParticleSystem> _particlesOnGrab;
        [SerializeField] private List<MonoBehaviour> _toDisableOnGrab;
        [SerializeField] private List<GameObject> _toHideOnGrab;
        [SerializeField] private List<Rigidbody> _dropParts; 
        
        
        public override void GrabBy(Transform hand, Action callback)
        {
            foreach (var mb in _toDisableOnGrab)
                mb.enabled = false;
            foreach (var pp in _particlesOnGrab)
            {
                pp.gameObject.SetActive(true);
                pp.Play();
            }
            foreach (var rb in _dropParts)
            {
                rb.isKinematic = false;
                rb.transform.parent = null;
            }
            foreach (var go in _toHideOnGrab)
                go.SetActive(false);   
            
            base.GrabBy(hand, callback);
        }
    }
}