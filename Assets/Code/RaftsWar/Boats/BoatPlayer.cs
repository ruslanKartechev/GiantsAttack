using System;
using RaftsWar.UI;
using SleepDev;
using UnityEngine;
using UnityEngine.Assertions;

namespace RaftsWar.Boats
{
    public class BoatPlayer : Boat, ITeamPlayer
    {
        public event Action<ITeamPlayer> OnDied;

        [Space(10)] 
        [SerializeField] private GameObject _captainGo;
        [SerializeField] private BoatHealth _health;
        [SerializeField] private CameraPointsManger _cameraPoints;
        [SerializeField] private Transform _deadCameraPoint;
        [SerializeField] private BoatFullIndicator _fullIndicator;

        private bool _isActive;
        private bool _isDead;
        private JoystickWithUI _joystickController;
        private BoatSettings _boatSettings;
        private IBoatCaptain _captain;
        private IBoatDeathEffect _boatDeathEffect;
        private ITeamUnitUI _teamUnitUI;
        private IUIDamagedEffect _uiDamaged;

        public Transform DeadCameraPoint => _deadCameraPoint;
        public CameraPointsManger CameraPointsManger => _cameraPoints;
        public ITarget Target => RootPart;
        public bool IsDead => _isDead;

        public void Init(Team team, BoatSettings boatSettings, PlayerCameraPointsSettings cameraPointsSettings)
        {
            CLog.Log($"[PlayerBoat] Init");
            _boatSettings = boatSettings;
            InitBoat(_boatSettings, team, _health);
            Team = team;
            _captain = _captainGo.GetComponent<IBoatCaptain>();
            _health.Init(boatSettings.health);
            _health.OnDied += DieFromDamage;
            _health.OnDamaged += OnDamaged;
            TeamsTargetsManager.Inst.AddPlayer(this);
            GetDeathEffect();
            _canConnect = false;
            _cameraPoints.InitCameraPoints(cameraPointsSettings);
        }

        public void SetTeamUnitUI(ITeamUnitUI ui)
        {
            _teamUnitUI = ui;
            _teamUnitUI.SetName(Team.BoatName, Team.TowerSettings.uiColor);
            _teamUnitUI.SetHealth(100f);
            _teamUnitUI.Show();
        }
        
        public void SetPlayerUI(IGameplayMenu menu)
        {
            InitStickController(menu);
            _uiDamaged = menu.DamagedEffect;
        }
        
        public void StopPlayer()
        {
            _joystickController.Stop();
            _health.DisplayOff();
            StopUnloadCheck();
            Kinematic(true);
        }
        
        public void ActivatePlayer()
        {
            _health.CanDamage = true;
            _joystickController.Activate();
            _cameraPoints.StartCameraFollow();
            _canConnect = true;
            StartUnloadCheck();
            _isActive =true;
        }
        
        public void UpdateCameraPoint()
        {
            _cameraPoints.SetCameraForCount(Parts.Count, GlobalConfig.PlayerCameraSetTime);
        }
        
        public void ReturnCameraToPlayer()
        {
            _cameraPoints.SetCameraForCount(Parts.Count, GlobalConfig.PlayerCameraReturnTime);
        }
        
        [ContextMenu("Kill")]
        public void Kill()
        {
            if (_isDead)
                return;
            CLog.Log($"[PlayerBoat] Killed");
            _isDead = true;
            _health.CanDamage = false;
            StopPlayer();
            _boatDeathEffect.Die();
            _teamUnitUI.Die();
            DamagedEffect.Stop();
            TeamsTargetsManager.Inst.RemovePlayer(this);
            _fullIndicator.Stop();
            ShakeCamera();
            HapticPlayer.HapticStrong();
            OnDied?.Invoke(this);
        }
        
        public override void HandleCollisionPushback(IBoat anotherBoat)
        {
            if(!_isActive)
                return;
            base.HandleCollisionPushback(anotherBoat);
            var damageAmount = (anotherBoat.Parts.Count + 1) * GlobalConfig.CollisionDamageMultiplier;
            DamageTarget.TakeDamage(new DamageArgs(transform.position, damageAmount));
            DropHalfOfAllParts();
            ShakeCamera();
            HapticPlayer.HapticMin();
        }
        
        protected override void HandleCollisionPushBackEnd()
        {
            base.HandleCollisionPushBackEnd();
        }

        protected override void HandlePartUnloaded()
        {
            UpdateCameraPoint();
        }

        protected override void HandleBoatPartConnected()
        {
            if(_parts.Count == GlobalConfig.PlayerBoatMaxConnectionsCount)
                _fullIndicator.Play();
            UpdateCameraPoint();
            HapticPlayer.HapticMin();
        }

        protected override void HandleNonRootBomb()
        {
            base.HandleNonRootBomb();
            ShakeCamera();
            _health.TakeDamage(new DamageArgs(transform.position, GlobalConfig.BombDamage));
        }
        
        protected override void HandleRootBomb()
        {
            Kill();
        }
        
        protected override bool TryConnectBP(BoatPart newPart)
        {
            if (_parts.Count >= GlobalConfig.PlayerBoatMaxConnectionsCount)
            {
                // CLog.Log($"Player boat is full");
                _fullIndicator.Play();
                return false;
            }
            return BoatUtils.ConnectToBoat(this, newPart);
        }

        
        private void InitStickController(IGameplayMenu menu)
        {
            _joystickController = gameObject.AddComponent<JoystickWithUI>();
            _joystickController.Sensitivity = GCon.GlobalConfig.joystickSensitivity;
            _joystickController.MaxRad = GCon.GlobalConfig.joystickRad;
            
            _joystickController.Init(menu.JoystickUI, menu.InputButton);
            _joystickController.OnDownCallback = OnDown;
            _joystickController.OnUpCallback = OnUp;
            _joystickController.OnXZMoveCallback = OnMove;
        }
        
        private void OnDamaged()
        {
            _teamUnitUI.UpdateHealth(_health.Percent100);
            _uiDamaged.PlayShort();
            DamagedEffect.Play();
        }
        
        private void OnUp()
        { 
            _captain.OnControlRelease();
            _rb.velocity = Vector3.zero;
        }

        private void OnDown()
        { }

        private void OnMove(Vector3 normalizedDirVec)
        {
            MoveBoat(normalizedDirVec);
            _captain.Rotate(normalizedDirVec);
        }

        private void DieFromDamage(IDamageable damageable)
        {
            damageable.OnDied -= DieFromDamage;
            Kill();
        }
        
        private void ShakeCamera()
        {
            _cameraPoints.Camera.Shake();
        }

        private void GetDeathEffect()
        {
            _boatDeathEffect = GetComponent<IBoatDeathEffect>();
#if UNITY_EDITOR
            Assert.IsNotNull(_boatDeathEffect, "Death effect on enemy boat is null");
#endif
            _boatDeathEffect.Boat = this;
            _boatDeathEffect.Captain = _captain;
        }

        
        
        
        
        

#if UNITY_EDITOR
        [ContextMenu("E_Die")]
        public void E_Die()
        {
            Kill();
        }

        [ContextMenu("Debug State")]
        public void E_DebugState()
        {
            var msg = "--------- Player boat state -------";
            msg += $"Can connect: {_canConnect}\n";
            msg += $"Can move: {CanMove}\n";
            msg += $"Is sending: {_isSending}\n";
            CLog.LogGreen(msg);
        }

        public void E_Push()
        {
            var center = transform.position;
            var size = 5f;
            var upLeft = new Vector3(center.x - size, center.y, center.z + size);
            var upRight = new Vector3(center.x + size, center.y, center.z + size);
            var botLeft = new Vector3(center.x - size, center.y, center.z - size);
            var botRight = new Vector3(center.x + size, center.y, center.z - size);
            var square = new Square(upLeft, upRight, botLeft, botRight);
            PushOutFromBlockedArea(square, transform);
        }

        public void E_CollisionPushback()
        {
            var pos = transform.position - transform.forward;
            HandleCollisionPushback(this);
        }
#endif
        
    }
}