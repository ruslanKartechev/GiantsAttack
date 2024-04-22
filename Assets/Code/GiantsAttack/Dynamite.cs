﻿using DG.Tweening;
using UnityEngine;

namespace GiantsAttack
{
    public class Dynamite : MonoBehaviour
    {
        [SerializeField] private float _finalScale;
        [SerializeField] private ParticleSystem _explsionParticles;
        [SerializeField] private GameObject _model;
        [SerializeField] private Rigidbody _rb;

        public void Drop()
        {
            gameObject.SetActive(true);
            _rb.isKinematic = false;
        }
        
        public void ScaleUp(float time, Ease ease)
        {
            gameObject.SetActive(true);
            transform.localScale = Vector3.zero;
            transform.DOScale(_finalScale * Vector3.one, time).SetEase(ease);
        }

        public void Explode()
        {
            _explsionParticles.Play();
            _model.SetActive(false);
            _rb.isKinematic = true;
        }
    }
}