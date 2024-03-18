using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class Boat : MonoExtended, IBoat
    {
        [SerializeField] protected BoatUnitsControlSettingsSO _unitsControlSettings;
        [SerializeField] protected Transform _movable;
        [SerializeField] protected Transform _partsParent;
        [SerializeField] protected BoatPart _rootPart;
        [SerializeField] protected Collider _pointCheckBox;
        [SerializeField] protected List<BoatPart> _parts;
        [SerializeField] protected Rigidbody _rb;

        protected BoatSettings _settings;
        protected IBoatViewSettings _viewSettings;
        protected TrackPointsKeeper _trackPointsKeeper;
        protected Coroutine _unloadAreaChecking;
        protected Coroutine _processing;
        protected HashSet<Collider> _connectedColliders = new HashSet<Collider>(20);

        private double _lastCollisionTime;
        private const double MinDelayBetweenCollisionPushback = .2f;
        protected List<BoatUnitController> _units = new List<BoatUnitController>(20);

        protected bool _isSending;
        protected float _speed;
        protected bool _canConnect;
        
        public bool CanMove { get; set; } = true;
        public float Speed => _speed;
        public BoatPart RootPart => _rootPart;
        public HashSet<Collider> ConnectedColliders => _connectedColliders;
        public TrackPointsKeeper PointsKeeper => _trackPointsKeeper;
        public IList<BoatPart> Parts => _parts;
        public IBoatViewSettings ViewSettings => _viewSettings;
        public Team Team { get; set; }
        public IDamageable DamageTarget { get; set; }
        public IBoatDamagedEffect DamagedEffect { get; set; }
        public Vector3 OverlapCheckBox => _pointCheckBox.bounds.extents;
        public Transform PartsParent => _partsParent;
        public List<BoatUnitController> Units => _units;

        public void RemoveUnit(BoatUnitController unit)
        {
            _units.Remove(unit);
        }

        public void InitBoat(BoatSettings settings, Team team, IDamageable damageTarget)
        {
            Team = team;
            DamageTarget = damageTarget;
            _viewSettings = team.BoatView;
            _settings = settings;
            _rootPart.Type = TargetType.Captain;
            _speed = _settings.moveSpeed;
            BoatUtils.AssignPartToBoat(this, _rootPart);
            _parts.Remove(_rootPart);
            InitTrackPoints();
            GetDamagedEffect();
        }

        public void CheckSpeed()
        {
            var t = (float)_parts.Count / _settings.countForMinSpeed;
            _speed = Mathf.Lerp(_settings.moveSpeed, _settings.minSpeed, t);
        }
        
        public void PartTriggerHandler(BoatPart fromPart, Collider other)
        {
            if (_connectedColliders.Contains(other))
                return;
            Interact(fromPart, other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_connectedColliders.Contains(collision.collider))
                return;
            TryCollideWithBoat(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_connectedColliders.Contains(other))
                return;
            switch (other.tag)
            {
                case GlobalConfig.BoatPartTag:
                    InteractWithBoatPart(other);

                    break;
                case GlobalConfig.ObstacleTag:
                    InteractWithObstacle(BoatUtils.GetClosestBoatPart(this, other.transform.position), other);
                    break;
            }
        }

        /// <summary>
        /// Expects direction to be NORMALIZED
        /// </summary>
        public bool MoveBoat(Vector3 direction)
        {
            if (!CanMove)
                return false;
            _rb.velocity = direction * (_speed );
            return false;

            #region No RB
            var oldPos = _movable.position;
            var newPos = oldPos + direction * (_speed * Time.deltaTime);
            var displacement = newPos - oldPos;
            
            var rot = _rootPart.transform.rotation;
            var size = OverlapCheckBox;
            var points = new List<Vector3>(_trackPointsKeeper.Map.Count);
            foreach (var pair in _trackPointsKeeper.Map)
                points.Add(pair.Key.position + displacement);
            for (var i = 0; i < points.Count; i++)
            {
                var pos = points[i];
                var overlaps = Physics.OverlapBox(pos, size, rot);
                foreach (var coll in overlaps)
                {
                    if (coll.gameObject.layer == GlobalConfig.BlockedLayer)
                        return false;
                    if (_connectedColliders.Contains(coll))
                        continue;
                }
            }
            _movable.position = newPos;
            return true;
            #endregion
        }
        
        public void PushOutFromBlockedArea(Square outOfSquare, Transform root)
        {
            CLog.Log($"{gameObject.name} Pushed out from area");
            StopRB();
            HandlePushOutStart();
            transform.DOKill();
            var outPoint = BoatUtils.GetPushOutPoint(outOfSquare, this, root);
            transform.DOMove(outPoint, .4f).OnComplete(HandlePushOutEnded);
        }
        
        /// <summary>
        /// Break all parts into pieces, hides the original and instantiates the broken version
        /// </summary>
        public void BreakAllParts()
        {
            if(_parts.Count > 0)
                BreakPartsRecursive(_parts[0]);
        }

        public void ResetConnectAbility()
        {
            _canConnect = true;
        }

        /// <summary>
        /// NOT USED ANYMORE
        /// </summary>
        public void UnloadToReceiver(IBoatPartReceiver receiver)
        {
            if (_parts.Count == 0)
            {
                HandleAllPartsUnloaded();
                return;
            }
            StopUnloading();
            _processing = StartCoroutine(Unloading(receiver));
            HandleStartedUnloading();
        }

        public bool ConnectNewBP(BoatPart newPart)
        {
            if (!_canConnect || _isSending)
                return false;
            if (TryConnectBP(newPart) == false)
            {
                // CLog.LogRed($"[Boat {gameObject.name}] Error, cannot connect new raft to boat");
                return false;
            }
            CheckSpeed();
            if (newPart.HasUnit)
                AddUnit(newPart.Unit);
            HandleBoatPartConnected();
            return true;
        }

        protected void AddUnit(BoatUnitController unit)
        {
            _units.Add(unit);
            unit.Unit.Init(Team);
            unit.Unit.Damage = _unitsControlSettings.settings.damage;
            unit.Unit.FireRate = _unitsControlSettings.settings.fireRate;
            unit.Radius = _unitsControlSettings.settings.radius;
            unit.Activate();
        }

        public virtual void HandleCollisionPushback(IBoat anotherBoat)
        {
            // CLog.LogRed($"{gameObject.name} Collision pushback");
            if (Time.time - _lastCollisionTime < MinDelayBetweenCollisionPushback)
                return;
            _lastCollisionTime = Time.time;
            StopProcess();
            _processing = StartCoroutine(PushbackJump(anotherBoat.RootPart.Point.position));
        }

        public void PlayDamaged()
        {
            DamagedEffect.Play();
        }

        #region VIRTUAL FOR OVERRIDE IN INHERITED
        protected virtual bool TryConnectBP(BoatPart newPart)
        {
            return BoatUtils.ConnectToBoat(this, newPart);
        }
        
        protected virtual void HandleBoatPartConnected()
        { }

        protected virtual void HandleNonRootBomb()
        {
            DropAllParts();
        }
        
        protected virtual void HandleRootBomb()
        { }
        
        protected virtual void HandleAllPartsUnloaded()
        { }

        protected virtual void HandlePartUnloaded()
        {
        }
        
        protected virtual void HandleStartedUnloading()
        { }
        
        protected virtual void HandleCollisionPushBackEnd()
        {
            CanMove = true;
            _canConnect = true;
        }
        
        protected virtual void HandlePushOutStart()
        { 
            StopUnloading();
            StopProcess();
            _canConnect = false;
            CanMove = false;
        }

        protected virtual void HandlePushOutEnded()
        {
            CanMove = true;
            _canConnect = true;
        }
#endregion

        #region Initialization
        private void InitTrackPoints()
        {
            _trackPointsKeeper = new TrackPointsKeeper(transform);
            _trackPointsKeeper.AddFirstPoint(_rootPart);
        }

        private void GetDamagedEffect()
        {
            DamagedEffect = GetComponent<IBoatDamagedEffect>();
            DamagedEffect.Boat = this;
        }
#endregion

        #region Interaction
                
        // returns if the boat can continue moving or not
        protected void Interact(BoatPart fromPart, Collider other)
        {
            switch (other.tag)
            {
                case GlobalConfig.BoatPartTag:
                     InteractWithBoatPart(other);
                     break;
                case GlobalConfig.ObstacleTag:
                    InteractWithObstacle(fromPart, other);
                    break;
            }
        }

        private void TryCollideWithBoat(GameObject go)
        {
            // assume go is the Rigidbody on the boat
            if (!go.TryGetComponent<IBoat>(out var boat))
                return;
            BoatsCollisionResolver.Inst.AddPair(this, boat);
        }

        private void InteractWithBoatPart(Collider other)
        {
            // assume trigger with trigger at GetChild(0) of the raft hierarchy
            if (!other.transform.parent.TryGetComponent<BoatPart>(out var otherPart)) 
                return;
            if(!otherPart.enabled)
                return;
            if (otherPart.IsAvailable)
                ConnectNewBP(otherPart);
        }

        private void InteractWithObstacle(BoatPart part, Collider other)
        {
            if (other.gameObject.TryGetComponent<IBoatObstacle>(out var obstacle))
            {
                obstacle.Hit();
                if (part != _rootPart)
                    HandleNonRootBomb();
                else
                    HandleRootBomb();
            }
        }

        protected void DropHalfOfAllParts()
        {
            var center = _rootPart.Point.position;
            var dropCount = 0;
            switch (_parts.Count)
            {
                case 0:
                    return;
                case 1:
                    dropCount = 1;
                    break;
                default:
                    dropCount = _parts.Count / 2;
                    break;
            }
            const float time = .33f;
            while(dropCount > 0)
            {
                var part = _parts[^1];
                var dir = (part.Point.position - center).XZPlane();
                var pos = center + dir * (GetPushDist());
                BoatUtils.UnlinkPartFromBoat(part, this);
                BoatUtils.SetBoatPartDefaultVisuals(part);
                part.DropAndPush(pos, time);
                dropCount--;
            }

            float GetPushDist()
            {
                return UnityEngine.Random.Range(2f, 3f);
            }
        }
        
        protected void DropAllParts()
        {
            CLog.Log($"[{gameObject.name}] Break all parts off");
            var center = _rootPart.Point.position;
            const float time = .33f;
            for (var ind = _parts.Count-1; ind >= 0; ind--)
            {
                var part = _parts[ind];
                var dir = (part.Point.position - center).XZPlane();
                var pos = center + dir * (GetPushDist());
                BoatUtils.UnlinkPartFromBoat(part, this);
                BoatUtils.SetBoatPartDefaultVisuals(part);
                part.DropAndPush(pos, time);
            }

            float GetPushDist()
            {
                return UnityEngine.Random.Range(2f, 3f);
            }
        }
#endregion


        protected void StopRB()
        {
            _rb.velocity = Vector3.zero;
        }

        protected void Kinematic(bool yes)
        {
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = yes;
        }

        private IEnumerator PushbackJump(Vector3 fromPoint)
        {
            CanMove = false;
            _canConnect = false;
            var settings = GlobalConfig.BoatPushSettings;
            GetPushbackPosition(fromPoint, settings, out var p3);
            _movable.DOKill();
            StopRB();
            var localPos = _movable.localPosition;
            localPos.y = 0f;
            _movable.localPosition = localPos;
            _movable.DOLocalJump(p3, settings.height, 1, settings.time);
            yield return new WaitForSeconds(settings.time);
            yield return null;
            HandleCollisionPushBackEnd();
        }

        private void GetPushbackPosition(Vector3 fromPoint, BoatPushSettings pushSettings, out Vector3 p3)
        {
            var p1 = _movable.localPosition;
            var dir = (p1 - fromPoint).normalized;
            p3 = p1 + dir * pushSettings.distance;
            p3.y = _movable.parent.position.y;
            if (BoatUtils.CheckIfWalkable(p3, this) == false)
                p3 = p1;
        }
        
        private void BreakPartsRecursive(BoatPart part)
        {
            BreakOff(part);
            var notConnected = new List<BoatPart>(_parts.Count);
            foreach (var pp in _parts)
            {
                if(BoatUtils.CheckBPConnectionToRoot(pp, _rootPart) == false)
                    notConnected.Add(pp);
            }
            foreach (var notConnectedPart in notConnected)
            {  
               // CLog.LogGreen($"Not connected {notConnectedPart.gameObject.name}");
               BreakOff(notConnectedPart);
            }

            void BreakOff(BoatPart pp)
            {
                BoatUtils.UnlinkPartFromBoat(pp, this);
                pp.BreakIntoPieces();
                // pp.WasJustDestroyed = true;   
            }
        }

        protected void StopProcess()
        {
            if(_processing != null)
                StopCoroutine(_processing);
        }

        protected void StopUnloading()
        {
            _isSending = false;
            StopProcess();
        }

        protected void StartUnloadCheck()
        {
            StopUnloadCheck();
            _unloadAreaChecking = StartCoroutine(CheckingUnloadArea());
        }

        protected void StopUnloadCheck()
        {
            if(_unloadAreaChecking != null)
                StopCoroutine(_unloadAreaChecking);
        }

        protected IEnumerator Unloading(IBoatPartReceiver receiver)
        {
            while (_parts.Count > 0)
            {
                var part = _parts[^1];
                BoatUtils.UnlinkPartFromBoat(part, this);
                receiver.TakeBoatPart(part);
                CheckSpeed();
                HandlePartUnloaded();
                yield return null;
                yield return null;
            }
            HandleAllPartsUnloaded();
        }
        
        private IEnumerator CheckingUnloadArea()
        {
            yield return null;
            IBoatPartReceiver receiver = Team.CurrentPartReceiver;
            var areaSquare = receiver.GetAreaSquare();
            const int skippedFrames = 5;
            while (true)
            {
                var isInside = false;
                var pos = RootPart.Point.position.ToXZPlane();
                isInside = areaSquare.CheckIfInside(pos);
                if (!isInside)
                {
                    foreach (var p in _parts)
                    {
                        pos = p.Point.position.ToXZPlane();
                        isInside = areaSquare.CheckIfInside(pos);
                        if (isInside)
                            break;
                    }
                }
                isInside &= _parts.Count > 0;
                isInside &= receiver.CanTake();
                if (isInside)
                {
                    var part = _parts[^1];
                    if (part.IsMoving == false)
                    { 
                        BoatUtils.UnlinkPartFromBoat(part, this);
                        receiver.TakeBoatPart(part);
                        CheckSpeed();
                        HandlePartUnloaded();
                        for (var k = 0; k < skippedFrames; k++)
                            yield return null;
                    }
                }
                yield return null;
                receiver = Team.CurrentPartReceiver;
                areaSquare = receiver.GetAreaSquare();
                yield return null;
            }
        }



        #region Editor Only

#if UNITY_EDITOR
        [ContextMenu("E_LogStructure")]
        public void E_LogStructure()
        {
            var ind = 0;
            foreach (var part in _parts)
            {
                var msg = "";
                var connectedCount = part.OtherSides.Count;
                msg += $"Part[{ind}] Count: {connectedCount} ";
                CLog.LogRed(msg);
                msg = "";
                var si = 0;
                foreach (var side in part.OtherSides)
                {
                    var name = side.BoatPart.gameObject.name;
                    msg += $"Neighbour {si}: PartName {name} ||  ";
                }
                CLog.LogWhite(msg);
                ind++;
            }
        }

        public void E_BreakOffLast()
        {
            if (_parts.Count == 0)
            {
                CLog.LogRed($"No parts connected");
                return;
            }
            var fromPart = _parts[1];
            BreakPartsRecursive(fromPart);
        }
        #endif
#endregion
    }

}










