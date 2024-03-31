﻿using System.Collections;
using GameCore.UI;
using GameCore.Core;
using SleepDev;
using SleepDev.Utils;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterAimer : MonoBehaviour, IHelicopterAimer
    {
        [SerializeField] private Transform _body;
        [SerializeField] private Transform _arm;
        [Header("xMin, xMax, yMin, yMax")]
        [SerializeField] private Vector4 _armAngleLimits;
        [SerializeField] private Vector3 _localAngleLimits;
        
        private float _screenLimitPercent = .05f;
        private Vector4 _screenLimits; // xMin, xMax, yMin, yMax;

        private IHelicopterShooter _shooter;
        private Vector3 _pointerPos;
        private Coroutine _aiming;
        private IControlsUI _controlsUI;
        private Coroutine _inputLoop;
        private bool _isDown;
        private bool _isAiming;

        public AimerSettings Settings { get; set; }
        public IAimUI AimUI { get; set; }
        
        public void Init(AimerSettings settings, IHelicopterShooter shooter, IControlsUI controlsUI, IAimUI aimUI)
        {
            Settings = settings;
            _shooter = shooter;
            AimUI = aimUI;
            _controlsUI = controlsUI;
            // SetPointerToCenter();
            _pointerPos = AimUI.GetScreenPos();
        }

        public void BeginAim()
        {
            if (_isAiming)
                return;
            _isAiming = true;
            CLog.Log($"[HeliAimer] Begin aim");
            _screenLimits.x = Screen.width * _screenLimitPercent;
            _screenLimits.y = Screen.width * (1 - _screenLimitPercent);
            _screenLimits.z = Screen.height * _screenLimitPercent;
            _screenLimits.w = Screen.height * (1 - _screenLimitPercent);
            Sub(false);
            Sub(true);
            AimUI.Show(true);
        }
        
        public void StopAim()
        {
            if (!_isAiming)
                return;
            _isAiming = false;
            CLog.Log($"[HeliAimer] Stop aim");
            Sub(false);
            _isDown = false;
            StopLoop();
            AimUI.StopRotation();
            AimUI.Hide(true);
        }

        public void SetInitialRotation()
        {
            RotateArm();
            SetPointerToCenter();
            RotateShooter();
        }
        
        public void Reset()
        { }

        private void SetPointerToCenter()
        {
            _pointerPos = new Vector3(Screen.width * .5f, Screen.height * .5f, 0f);
        }
        
        private void Sub(bool sub)
        {
            if (sub)
            {
                _controlsUI.InputButton.OnUp += OnUp;
                _controlsUI.InputButton.OnDown += OnDown;
            }
            else
            {
                _controlsUI.InputButton.OnUp -= OnUp;
                _controlsUI.InputButton.OnDown -= OnDown;
            }
        }

        private void OnDown()
        {
            _isDown = true;
            StartLoop();
            _shooter.BeginShooting();
            AimUI.BeginRotation(Settings.aimRotSpeed);
        }

        private void OnUp()
        {
            _isDown = false;
            StopLoop();
            _shooter.StopShooting();
            AimUI.StopRotation();
        }

        private void StopLoop()
        {
            if(_inputLoop != null)
                StopCoroutine(_inputLoop);
        }

        private void StartLoop()
        {
            StopLoop();
            _inputLoop = StartCoroutine(Aiming());
        }

        private void RotateToDelta(Vector3 delta)
        {
            delta *= Settings.sensitivity / 100;
            var rot = _body.rotation;
            rot *= Quaternion.Euler(-delta.y, delta.x, 0f);
            _body.rotation = rot;
            _body.ClampLocalRotation(_localAngleLimits);
            _pointerPos = AimUI.GetScreenPos();
            _shooter.RotateToScreenPos(_pointerPos);
            RotateArm();
        }
        
  
        
        private void RotateShooter()
        {
            _shooter.RotateToScreenPos(_pointerPos);
        }

        private void RotateArm()
        {
            var angles = _body.localEulerAngles;
            angles = angles.AnglesTo180();
            var xt = Mathf.InverseLerp(-45, 45, angles.x);
            var yt = Mathf.InverseLerp(-45, 45, angles.y);
            var ax = Mathf.Lerp(_armAngleLimits.x, _armAngleLimits.y, xt);
            var az = -Mathf.Lerp(_armAngleLimits.z, _armAngleLimits.w, yt);
            _arm.localRotation = Quaternion.Euler(ax, 0f, az);
        }
        
        private IEnumerator Aiming()
        {
            var inp = GCon.Input;
            var pos = inp.MousePosition();
            var oldPos = pos;
            yield return null;
            while (true)
            {
                pos = inp.MousePosition();
                var delta = pos - oldPos;
                delta *= Settings.sensitivity;
                RotateToDelta(delta);
                oldPos = pos;
                yield return null;
            }
        }
    }
}