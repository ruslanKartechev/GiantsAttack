using System.Collections;
using GameCore.UI;
using GameCore.Core;
using SleepDev;
using SleepDev.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

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

        private Coroutine _aiming;
        private Coroutine _rotating;
        private IHelicopterShooter _shooter;
        private Vector3 _pointerPos;
        private IControlsUI _controlsUI;
        private Coroutine _inputLoop;
        private bool _isDown;
        private bool _isAiming;
        
        private Vector3 _targetPointerPos;
        private Quaternion _targetRot;

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
            _targetRot = _body.localRotation;
            _targetPointerPos =  AimUI.GetScreenPos();
        }

        public void BeginAim()
        {
            if (_isAiming)
                return;
            _isAiming = true;
            CLog.Log($"[HeliAimer] Begin aim");
            Sub(false);
            Sub(true);
            AimUI.Show(true);
            _targetRot = _body.localRotation;
            _pointerPos = _targetPointerPos;
            
            if(_rotating != null)
                StopCoroutine(_rotating);
            _rotating = StartCoroutine(Rotating());
            if (Input.GetMouseButton(0))
            {
                OnDown();
                _upInputWaiting = StartCoroutine(WaitingForUpInput());
            }
        }

        private Coroutine _upInputWaiting;
        private void StopWaitingForUpInput()
        {
            if(_upInputWaiting != null)
                StopCoroutine(_upInputWaiting);
        }

        private IEnumerator WaitingForUpInput()
        {
            while (true)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    OnUp();
                    yield break;
                }
                yield return null;
            }
        }
        
        public void StopAim()
        {
            if (!_isAiming)
                return;
            _isAiming = false;
            CLog.Log($"[HeliAimer] Stop aim");
            
            _isDown = false;
            Sub(false);
            StopLoop();
            if(_rotating != null)
                StopCoroutine(_rotating);
            _pointerPos = _targetPointerPos;
            AimUI.SetPosition(_targetPointerPos);
            AimUI.StopRotation();
            AimUI.Hide(true);
        }

        public void SetInitialRotation()
        {
            RotateArm();
            SetPointerToCenter();
            RotateShooter();
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

        private void RotateToDelta(Vector3 delta)
        {
            var newPos = _pointerPos + delta.normalized * Settings.sensitivityUI;
            if ((newPos - _pointerPos).magnitude < Settings.aimUIMaxDiv)
                _pointerPos = newPos;
            delta *= Settings.sensitivity / 100;
            var rot = _targetRot;
            rot *= Quaternion.Euler(-delta.y, delta.x, 0f);
            rot *= Quaternion.Euler(UnityEngine.Random.insideUnitSphere * Settings.noiseMagnitude);
            _targetRot = rot;   
        }
        
        private void DEP_RotateToDelta(Vector3 delta)
        {
            _pointerPos += delta;
            delta *= Settings.sensitivity / 100;
            var rot = _body.rotation;
            rot *= Quaternion.Euler(-delta.y, delta.x, 0f);
            rot *= Quaternion.Euler(UnityEngine.Random.insideUnitSphere * Settings.noiseMagnitude);
            _body.rotation = rot;
            _body.ClampLocalRotation(_localAngleLimits);
            // _pointerPos = AimUI.GetScreenPos();
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

        private IEnumerator Rotating()
        {
            while (true)
            {
                var toRot = Quaternion.RotateTowards(
                    _body.localRotation, _targetRot,  Settings.rotCenterSpeed * Time.deltaTime);
                _body.localRotation = toRot;
                _body.ClampLocalRotation(_localAngleLimits);     
                RotateArm();
                
                var distVec = (_targetPointerPos - _pointerPos);
                if (distVec != Vector3.zero)
                {
                    var magn = distVec.magnitude;
                    var speed = magn < Settings.aimCenterSpeed ? magn : Settings.aimCenterSpeed;
                    _pointerPos += distVec / magn * speed;
                }
                AimUI.SetPosition(_pointerPos);
                _shooter.RotateToScreenPos(_pointerPos);
                yield return null;
            }
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
                RotateToDelta(delta);
                oldPos = pos;
                yield return null;
            }
        }
    }
}