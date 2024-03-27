using System.Collections;
using GameCore.UI;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterAimer : MonoBehaviour, IHelicopterAimer
    {
        [SerializeField] private Transform _body;
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
            SetPointerToCenter();
            RotateShooter();
        }
        
        public void Reset()
        {
            ResetPointerAndUI();
        }

        private void ResetPointerAndUI()
        {
            SetPointerToCenter();
            AimUI.SetPosition(_pointerPos);
        }

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

        private void MoveToDelta(Vector3 delta)
        {
            var nextPos = _pointerPos + delta;
            nextPos.x = Mathf.Clamp(nextPos.x, _screenLimits.x, _screenLimits.y);
            nextPos.y = Mathf.Clamp(nextPos.y, _screenLimits.z, _screenLimits.w);
            // CLog.Log($"Pointer pos: {nextPos}");
            _pointerPos = nextPos;
            AimUI.SetPosition(_pointerPos);
            RotateShooter();
        }

        private void RotateToDelta(Vector3 delta)
        {
            delta *= Settings.sensitivity / 100;
            var rot = _body.rotation;
            rot *= Quaternion.Euler(-delta.y, delta.x, 0f);
            _body.rotation = rot;
            _pointerPos = AimUI.GetScreenPos();
            _shooter.RotateToScreenPos(_pointerPos);
        }
        
        private void RotateShooter()
        {
            _shooter.RotateToScreenPos(_pointerPos);
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