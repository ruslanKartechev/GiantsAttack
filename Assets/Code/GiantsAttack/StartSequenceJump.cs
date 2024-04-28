using System;
using System.Collections.Generic;
using GameCore.Cam;
using SleepDev.Sound;
using UnityEngine;

namespace GiantsAttack
{
    public class StartSequenceJump : LevelStartSequence
    {
        public enum AnimType
        {
            JumpDown, KickUp
        }
        [SerializeField] private AnimType _animType;
        [SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private List<AnimatedTarget> _vehicles;
        [SerializeField] private List<StageListener> _listeners;
        [SerializeField] private float _pushBackForce;
        [SerializeField] private float _endCallbackDelay;
        [SerializeField] private SoundSo _groundHitSound;
        // [SerializeField] private Transform _torqueDir;
        [SerializeField] private float _torqueForce;
        
        private Action _callback;

        #if UNITY_EDITOR
        public override void E_Init()
        {
        }
        #endif

        public override void Begin(Action onEnd)
        {
            _callback = onEnd;
            foreach (var ll in _listeners)
                ll.OnActivated();
            switch (_animType)
            {
                case AnimType.JumpDown:
                    Enemy.Jump(false);
                    break;
                case AnimType.KickUp:
                    Enemy.KickUp();
                    break;
            }
            Enemy.AnimEventReceiver.EOnJumpDown += OnJumped;
        }

        private void OnJumped()
        {
            foreach (var ll in _listeners)
                ll.OnCompleted();
            Enemy.AnimEventReceiver.EOnJumpDown -= OnJumped;
            CameraContainer.Shaker.PlayDefault();
            _jumpParticles.gameObject.SetActive(true);
            _jumpParticles.transform.position = Enemy.Point.position;
            _jumpParticles.Play();
            foreach (var v in _vehicles)
            {
                var vec = (v.transform.position - Enemy.Point.position).normalized;
                var torque = Vector3.Cross(-vec, Vector3.up) * _torqueForce;
                v.StopMovement();
                v.ExplodeWithTorque(vec * _pushBackForce, torque);
            }
            _groundHitSound?.Play();
            Delay(_callback, _endCallbackDelay);
        }
    }
}