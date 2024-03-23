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

        public AimerSettings Settings { get; set; }
        public IAimUI AimUI { get; set; }

        public void Init(AimerSettings settings, IHelicopterShooter shooter, IControlsUI controlsUI)
        {
            Settings = settings;
            _shooter = shooter;
            shooter.FromPoint = _fromPoint;
            shooter.AtPoint = _atPoint;
            _controlsUI = controlsUI;
        }

        public void BeginAim()
        {
            CLog.Log($"[HeliAimer] Begin aim");
            _screenLimits.x = Screen.width * _screenLimitPercent;
            _screenLimits.y = Screen.width * (1 - _screenLimitPercent);
            _screenLimits.z = Screen.height * _screenLimitPercent;
            _screenLimits.w = Screen.height * (1 - _screenLimitPercent);
            ResetPointer();
            _shooter.RotateToScreenPos(_pointerPos);       
            Sub(false);
            Sub(true);
        }
        
        public void StopAim()
        {
            CLog.Log($"[HeliAimer] Stop aim");
            Sub(false);
            _isDown = false;
        }

        public void Reset()
        {
            ResetPointer();
        }

        private void ResetPointer()
        {
            _pointerPos = new Vector3(Screen.width * .5f, Screen.height * .5f, 0f);
            AimUI.SetPosition(_pointerPos);
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