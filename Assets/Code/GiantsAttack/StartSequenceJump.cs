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
        [SerializeField] private MonsterController _enemy;
        [SerializeField] private ParticleSystem _jumpParticles;
        [SerializeField] private List<AnimatedVehicleBase> _vehicles;
        [SerializeField] private float _pushBackForce;
        [SerializeField] private float _endCallbackDelay;
        private Action _callback;

        #if UNITY_EDITOR
        public override void E_Init()
        {
            if (_enemy == null)
            {
                _enemy = FindObjectOfType<MonsterController>();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
        #endif

        public override void Begin(Action onEnd)
        {
            _callback = onEnd;
            switch (_animType)
            {
                case AnimType.JumpDown:
                    _enemy.Jump(false);
                    break;
                case AnimType.KickUp:
                    _enemy.KickUp();
                    break;
            }
            _enemy.AnimEventReceiver.OnJumpDown += OnJumped;
        }

        private void OnJumped()
        {
            _enemy.AnimEventReceiver.OnJumpDown -= OnJumped;
            CameraContainer.Shaker.PlayDefault();
            _jumpParticles.gameObject.SetActive(true);
            _jumpParticles.transform.position = _enemy.transform.position;
            _jumpParticles.Play();
            foreach (var v in _vehicles)
                v.ExplodeFromCenter(_enemy.transform.position, _pushBackForce);
            Delay(_callback, _endCallbackDelay);
        }
    }
}