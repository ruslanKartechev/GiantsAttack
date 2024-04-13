using System;
using System.Collections.Generic;
using GameCore.Cam;
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
        [SerializeField] private List<AnimatedVehicleBase> _vehicles;
        [SerializeField] private List<StageListener> _listeners;
        [SerializeField] private float _pushBackForce;
        [SerializeField] private float _endCallbackDelay;
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
            Enemy.AnimEventReceiver.OnJumpDown += OnJumped;
        }

        private void OnJumped()
        {
            Enemy.AnimEventReceiver.OnJumpDown -= OnJumped;
            CameraContainer.Shaker.PlayDefault();
            _jumpParticles.gameObject.SetActive(true);
            _jumpParticles.transform.position = Enemy.Point.position;
            _jumpParticles.Play();
            foreach (var v in _vehicles)
                v.ExplodeFromCenter(Enemy.Point.position, _pushBackForce);
            Delay(_callback, _endCallbackDelay);
        }
    }
}