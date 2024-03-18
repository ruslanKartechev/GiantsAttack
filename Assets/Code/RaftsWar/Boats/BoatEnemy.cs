using System;
using System.Collections;
using System.Collections.Generic;
using RaftsWar.UI;
using SleepDev;
using SleepDev.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace RaftsWar.Boats
{
    
    /// <summary>
    /// Behaviour:
    /// NextAction() -> decides to either collect go to another raft or go to unload
    /// OnCollisionWithAnotherBoat -> NextAction()
    /// OnBombed -> NextAction()
    /// OnUnloaded -> NextAction()
    /// When trying to move: !!!
    /// When going to collect a raft -> look for random raft. If cannot move there -> look for another and repeat up to n times.
    /// If failed to find a raft completely -> Move to random reachable point around (MoveToRandomPointAround()).
    /// When stuck during movement -> wait for n seconds. If still stuck -> MoveToRandomPointAround()
    /// When failed to build a path to unload point -> MoveToRandomPointAround();
    /// </summary>
    public class BoatEnemy : Boat, ITeamPlayer 
    {
        public event Action<ITeamPlayer> OnDied;

        [Space(10)]
        [SerializeField] private GameObject _captainGo;
        [SerializeField] private BoatHealth _health;
        [SerializeField] private Transform _deadCameraPoint;
        [SerializeField] private EnemyNavAgent _agent;
        private EnemyBehaviourPattern _pattern;
        private BoatSettings _boatSettings;
        private BoatPart _currentPursuedPart;
        private BoatPartsManager _partsManager;
        private Coroutine _moving;
        private Coroutine _movingToPoint;
        private EnemyAIData _aiData;
        private IBoatCaptain _captain;
        private IBoatDeathEffect _boatDeathEffect;
        private bool _isDead;
        private int _stuckInARow;
        private ITeamUnitUI _ui;

        private const int CriticalStuckToTeleport = 8;
        private const float MaxStuckTime = .65f;
        private const float UnloadTime = .5f;
        // private static readonly List<float> angles = new(){0, -45, 45, -90, 90};

        public ITarget Target => RootPart;
        public Transform DeadCameraPoint => _deadCameraPoint;
        public bool IsDead => _isDead;


        public void Init(Team team, BoatPartsManager partsManager, EnemyAIData aiData, EnemyBehaviourPattern pattern)
        {
            CLog.Log($"[EnemyBoat] Inited");
            Team = team;
            _partsManager = partsManager;
            _aiData = aiData;
            _boatSettings = team.BoatSettings;
            _pattern = pattern;
            InitBoat(_boatSettings, team, _health);
            
            _captain = _captainGo.GetComponent<IBoatCaptain>();
            _health.Init(_boatSettings.health);
            _captain.SetView(team.UnitsView);
            _health.OnDamaged += OnDamaged;
            _health.OnDied += DieFromDamage;
            TeamsTargetsManager.Inst.AddPlayer(this);
            GetDeathEffect();
            CanMove = false;
            _canConnect = false;
            _agent.On();
            _agent.AdjustRadius();
        }

        public void SetTeamUnitUI(ITeamUnitUI ui)
        {
            _ui = ui;
            _ui.SetName(Team.BoatName, Team.TowerSettings.uiColor);
            _ui.SetHealth(100f);
            _ui.Show();
        }

        public void ActivatePlayer()
        {
            if(GlobalConfig.EnemyNotActivated)return;
            CLog.Log($"[EnemyBoat] {gameObject.name} Activated");
            CanMove = true;
            _health.CanDamage = true;
            _canConnect = true;
            GoToCollectBoatPart();
            StartUnloadCheck();
        }

        public void StopPlayer()
        {
            StopDelayedAction();
            StopUnloading();
            StopMovingAgent();
            StopUnloadCheck();
            CanMove = false;
            Kinematic(true);
        }
        
        [ContextMenu("Kill")]
        public void Kill()
        {
            if (_isDead)
                return;
            _isDead = true;
            CLog.Log($"[Enemy {gameObject.name}] Killed");
            TeamsTargetsManager.Inst.RemovePlayer(this);
            DamagedEffect.Stop();
            UnsubTargetPart();
            _agent.Off();
            _health.CanDamage = false;
            _health.OnDamaged -= OnDamaged;
            _health.DisplayOff();
            _ui.Die();
            _boatDeathEffect.Die();
            StopAllCoroutines();
            Kinematic(true);
            OnDied?.Invoke(this);
            enabled = false;
        }

        protected override bool TryConnectBP(BoatPart newPart)
        {
            // CLog.Log($"[{gameObject.name}] enemy override connecting, count: {_parts.Count}");
            switch (_parts.Count)
            {
                case 0:
                    BoatUtils.ConnectBoatPartToRoot(this, newPart, Vector3.forward);
                    return true;
                case 1:
                    BoatUtils.ConnectBoatPartToRoot(this, newPart, Vector3.right);
                    return true;
                case 2:
                    BoatUtils.ConnectBoatPartToRoot(this, newPart, -Vector3.right);
                    return true;
                case 3:
                    BoatUtils.ConnectBoatPartToRoot(this, newPart, -Vector3.forward);
                    return true;
                default:
                    return false;
            }
            return false;
        }

        public override void HandleCollisionPushback(IBoat anotherBoat)
        {
            StopMoving();
            UnsubTargetPart();
            ResetStuckCount();
            DropHalfOfAllParts();
            base.HandleCollisionPushback(anotherBoat);
            var damageAmount = (anotherBoat.Parts.Count + 1) * 2f;
            DamageTarget.TakeDamage(new DamageArgs(transform.position, damageAmount));
        }

        protected override void HandleCollisionPushBackEnd()
        {
            base.HandleCollisionPushBackEnd();
            StopMoving();
            NextAction();
        }
        
        protected override void HandlePushOutStart()
        {
            StopMoving();
            base.HandlePushOutStart();
        }

        protected override void HandlePushOutEnded()
        {
            base.HandlePushOutEnded();
            NextAction();
        }

        private void ResetAndNextAction()
        {
            _stuckInARow = 0;
            NextAction();
        }
        
        private void NextAction()
        {
            // CLog.Log($"[EnemyBoat] nextAction(), stuck count {_stuckInARow}");
            if (_isDead)
                return;
            if (_stuckInARow >= CriticalStuckToTeleport)
            {
                // CLog.LogRed("[EnemyBoat] Action: teleport (critically stuck)");
                TeleportToOutPoint();
                return;
            }
            if (Parts.Count >= _pattern.targetRaftsCount)
            {
                // CLog.Log($"[Enemy {gameObject.name}] Action: unload");
                MoveToUnload();
                return;
            }
            // CLog.Log($"[Enemy  {gameObject.name}] Action: collect more");
            GoToCollectBoatPart();
        }

        private BoatPart GetRandomCloseBoatPart()
        {
            BoatPart result = null;
            var count = 3;
            var mindD2 = float.MaxValue;
            var pos = _movable.position;
            for (var i = 0; i < count; i++)
            {
                var p = _partsManager.GetRandomAvailable();
                if (p == null)
                    break;
                var vec = p.Point.position - pos;
                var d2 = vec.x * vec.x + vec.z * vec.z;
                if (d2 <= mindD2)
                {
                    mindD2 = d2;
                    result = p;
                }
            }
            return result;
        }

        private void GoToCollectBoatPart()
        {
            const int maxIterations = 5;
            for (var i = 0; i < maxIterations; i++)
            {
                var randomPart = GetRandomCloseBoatPart();
                if (randomPart == null)
                {
                    // CLog.LogRed($"[EnemyBoat] random part is null, probably error. Try after 1s delay");
                    Delay(NextAction, 1f);
                    return;
                }
                var pos = randomPart.Point.position;
                if (TryMoveTo(pos, NextAction, MoveToRandomLocationAround) == false) // cannot move, continue loop to try moving to another point
                {
                    CLog.LogRed($"[EnemyBoat] trying to move to random part failed, calling again");
                }
                else // started moving, abandon
                {
                    _currentPursuedPart = randomPart;
                    _currentPursuedPart.OnBecameAvailable += OnPartBecameAvailable;
                    return;
                }
            }
            CLog.LogRed($"[EnemyBoat] Finding path to a random bp failed after loop...");
            // MoveToRandomLocationAround();
        }
        
        private void OnPartBecameAvailable(BoatPart part, bool available)
        {
            part.OnBecameAvailable -= OnPartBecameAvailable;
            if (part.HostBoat != this)
                NextAction();
        }
        
        private void OnDamaged()
        {
            // CLog.Log($"[Enemy_{gameObject.name}] Damaged");
            UpdateHealth();
            PlayDamaged();
            var dir = (_movable.position - _health.LastDamagedArgs.fromPoint).XZPlane();
            var pos = _movable.position + dir.normalized * _pattern.damagedRunDistance;
            TryMoveTo(pos, NextAction, MoveToRandomLocationAround);
        }
        
        private void UpdateHealth()
        {
            _ui.UpdateHealth(_health.Percent100);
        }
        
        private void TeleportToOutPoint()
        {
            CLog.LogWhite($"[Enemy_{gameObject.name}] Stuck... Teleporting to out point");
            StopMoving();
            
            UnsubTargetPart();
            var point = _aiData.outPoints.GetClosestPoint(_movable.position);
            var pos = point.position;
            pos.y = _movable.position.y;
            _movable.position = pos;
            ResetStuckCount();
            NextAction();
        }
        
        private void MoveToUnload()
        {
            if(_parts.Count == 0)
                NextAction();
            var unloadTarget = Team.Tower.UnloadPointsManager;
            if (Team.Tower.IsMaxLevel)
            {
                if (Team.Tower.Catapult == null)
                {
                    CLog.LogBlue($"[Enemy {gameObject.name}] Catapult is null, delay next call");
                    Delay(NextAction, 1f);
                    return;
                }
                CLog.LogBlue($"[Enemy {gameObject.name}] Unload point is set to the catapult");
                unloadTarget = Team.Tower.Catapult.UnloadPointsManager;
            }
            var unloadPoint = BoatUtils.GetUnloadPosition(this, unloadTarget, 
            _agent.Radius * 1.1f);
            if (TryMoveTo(unloadPoint,UnloadToOwn, MoveToRandomLocationAround) == false)
            {
                CLog.Log($"[Enemy {gameObject.name}] path to unload point failed, move to random");
                MoveToRandomLocationAround();
            }
        }

        private void UnloadToOwn()
        {
            CLog.LogGreen($"Unload to own call");
            UnloadToReceiver(Team.CurrentPartReceiver);
        }

        // Intended to be use to wait until parts are unloaded
        private void DelayedNextAction()
        {
            Delay(NextAction, .5f);
        }
        
        private IEnumerator WaitingForUnload()
        {
            yield return new WaitForSeconds(UnloadTime);
            NextAction();
        }


        private void MoveToRandomLocationAround()
        {
            var pos = _movable.position;
            int it = 1;
            foreach (var distance in BoatUtils.EnemyMoveAroundDistances)
            {
                var count = BoatUtils.Directions.Count;
                var total = count;
                var ind = UnityEngine.Random.Range(0, count);
                while (count > 0)
                {
                    var dir = BoatUtils.Directions[ind];
                    var endPos = pos + dir * distance;
                    if (TryMoveTo(endPos, ResetAndNextAction, MoveToRandomLocationAround))
                        return;
                    it++;
#if UNITY_EDITOR
                    Debug.DrawLine(pos, endPos, Color.blue, 1);
                    CLog.Log($"[Enemy {gameObject.name}] MoveAround Failed iteration {it}");
#endif
                    count--;
                    ind++;
                    if (ind >= total)
                        ind = 0;
                }
            }
            CLog.Log($"[Enemy {gameObject.name}] cannot find any outer direction");
            OnCriticalStuck();
        }

        private void OnCriticalStuck()
        {
            CLog.LogRed($"[Enemy {gameObject.name}] Critical problem => teleport to outer point");
            StopMovingAgent();
            TeleportToOutPoint();
            NextAction();
        }

        private bool TryMoveTo(Vector3 position, Action onEnd, Action onFailed = null)
        {
            Debug.DrawLine(transform.position, position, Color.black, 2f);
            _agent.SetSpeed(_speed);
            _agent.AdjustRadius();
            _agent.WarpTo(transform.position);
            var agentCanMove = _agent.MoveTo(position);
            if (!agentCanMove)
                CLog.LogRed($"[BoatEnemy] Agent cannot build a path to {position}");
            if (agentCanMove)
            {
                StopMoving();
                _moving = StartCoroutine(NavMovingToPoint(onEnd));
            }
            return agentCanMove;
        }
        
        private const float destinationThreshold2 = .5f * .5f;
        private const float warpThreshold = 10f;
        private const float tooCloseThreshold = 3f;      

        private IEnumerator NavMovingToPoint(Action onEnd)
        {
            yield return null;
           
            while (true)
            {
                var vec = (_agent.Position - transform.position).XZPlane();
                var magn = vec.magnitude;
                if (magn > 0)
                {
                    if (magn >= warpThreshold) // if the agent is too fat away
                    {
                        _agent.WarpTo(transform.position);
                    }
                    var speed = _speed;
                    if(magn <= tooCloseThreshold) // if the distance is close and we can overrun the agent
                        speed = 6f;
                    vec *= (speed / magn);
                    _rb.velocity = vec;
                    _captain.Rotate(vec);
                }
                if ((transform.position - _agent.CurrentDestination()).XZDistance2() <= destinationThreshold2)
                {
                    CLog.LogWhite($"[{gameObject.name}] Reached end point");
                    StopRB();
                    onEnd.Invoke();
                    yield break;
                }
                yield return null;
            }
        }
        
        private IEnumerator MovingOnNavMeshPath(NavMeshPath path, Action onEnd = null)
        {
            CLog.Log($"Path corners count {path.corners.Length}");
            for (var i = 1; i < path.corners.Length; i++)
            {
                var corner = path.corners[i];
#if UNITY_EDITOR
                // CLog.LogYellow($"Distance to next ({i}) corner {(corner - _movable.position).magnitude}");
                Debug.DrawLine(corner + Vector3.up, transform.position + Vector3.up, Color.black, 20f);
#endif
                yield return Moving(corner);
            }
            CLog.LogRed($"Path end reached");
            yield return null;
            onEnd.Invoke();
        }

        private IEnumerator Moving(Vector3 position)
        {
            var tr = _movable;
            var targetDistance = .5f;
            var dist = float.MaxValue;
            var projection = dist;
            var initialVec = position - tr.position;
            while (projection > targetDistance)
            {
                var dir = (position - tr.position).XZPlane();
                projection = Vector3.Dot(dir, initialVec);
                dir.Normalize();
                _captain.Rotate(dir);
                _rb.velocity = dir * (_speed);
                yield return new WaitForFixedUpdate();
            }
        }
        
        /// <summary>
        /// Does not stop the NAV AGENT movement, only stops boat-internal movement tracking
        /// </summary>
        private void StopMoving()
        {
            if(_moving != null)
                StopCoroutine(_moving);
            StopRB();
        }

        private void StopMovingAgent()
        {
            StopMoving();
            _agent.Stop();
        }
        
        protected override void HandleBoatPartConnected()
        {
            NextAction();
        }

        protected override void HandleNonRootBomb()
        {
            base.HandleNonRootBomb();
            NextAction();
            _health.TakeDamage(new DamageArgs(transform.position, GlobalConfig.BombDamage));
        }
        
        protected override void HandleRootBomb()
        {
            BreakAllParts();
            DropAllParts();
            NextAction();
            _health.TakeDamage(new DamageArgs(transform.position, GlobalConfig.BombDamage));
        }

        protected override void HandleStartedUnloading()
        {
            // CLog.Log($"[Enemy_{gameObject.name}] Unloading started");     
            StopMoving();
            _moving = StartCoroutine(WaitingForUnload());
        }

        protected override void HandlePartUnloaded()
        {
            base.HandlePartUnloaded();
            NextAction();
        }

        protected override void HandleAllPartsUnloaded()
        {
            ResetConnectAbility();
            StopMoving();
            NextAction();
        }
        
        private void DieFromDamage(IDamageable damageable)
        {
            damageable.OnDied -= DieFromDamage;
            Kill();
        }

        private void UnsubTargetPart()
        {
            if (_currentPursuedPart != null)
                _currentPursuedPart.OnBecameAvailable -= OnPartBecameAvailable;
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
        
        private void ResetStuckCount()
        {
            _stuckInARow = 0;
        }
        
                
    }
}