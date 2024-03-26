using System.Collections;
using GameCore.UI;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterAimer : MonoBehaviour, IHelicopterAimer
    {
        [SerializeField] private Transform _fromPoint;
        [SerializeField] private Transform _atPoint;
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
        
        public void Init(AimerSettings settings, IHelicopterShooter shooter, IControlsUI controlsUI)
        {
            Settings = settings;
            _shooter = shooter;
            shooter.FromPoint = _fromPoint;
            shooter.AtPoint = _atPoint;
            _controlsUI = controlsUI;
            SetPointerToCenter();
        }

        public void BeginAim()
        {
            if (_isAiming)
                return;
            _isAiming = true;
            SetPointerToCenter();
            CLog.Log($"[HeliAimer] Begin aim");
            _screenLimits.x = Screen.width * _screenLimitPercent;
            _screenLimits.y = Screen.width * (1 - _screenLimitPercent);
            _screenLimits.z = Screen.height * _screenLimitPercent;
            _screenLimits.w = Screen.height * (1 - _screenLimitPercent);
            Sub(false);
            Sub(true);
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
        }

        private void OnUp()
        {
            _isDown = false;
            StopLoop();
            _shooter.StopShooting();
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
            RotateShooter();
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
                MoveToDelta(delta);
                AimUI.SetPosition(_pointerPos);
                oldPos = pos;
                yield return null;
            }
        }
    }
}