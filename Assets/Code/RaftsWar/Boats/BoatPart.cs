using System;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using SleepDev.Pooling;
using UnityEngine;

namespace RaftsWar.Boats
{
    /// <summary>
    /// Building block of the boats. Mostly only holds the data. Most of the logic is in BoatUtils class
    /// Is Pooled. Returns itself to pool when destroyed or hidden or called back
    /// </summary>
    public class BoatPart : MonoBehaviour, IPooledObject<BoatPart>, ITarget
    {
        /// <summary>
        /// Event raised when the boat is connected, dropped or destroyed
        /// </summary>
        public event Action<BoatPart, bool> OnBecameAvailable;

        [SerializeField] private float _radius = 1f;
        [SerializeField] private List<BoatSide> _sides;
        [SerializeField] private Collider _collider;
        [SerializeField] private Collider _trigger;
        [SerializeField] private Transform _centerPoint;
        [SerializeField] private DamagePointsProvider _damagePoints;
        [SerializeField] private FloatingAnimator _floatingAnimator;
        [SerializeField] private Transform _scalable;
        [SerializeField] private BoatUnitController _unit;
        private bool _isMoving;
        private ArrowStuckTarget _arrowStuckManager;
        private Sequence _currentSequence;

        public List<BoatSide> OtherSides { get; set; } = new List<BoatSide>();
        public List<BoatSide> MyUsedSides { get; set; } = new List<BoatSide>();
        public TargetType Type { get; set; } = TargetType.Boat;
        public IList<BoatSide> Sides => _sides;

        public bool IsAvailable => HostBoat == null && _isMoving == false;
        public float Radius => _radius;
        public Collider Collider => _collider;
        public Collider Trigger => _trigger;
        public Transform Point => _centerPoint;
        public Transform Scalable => _scalable;
        public FloatingAnimator FloatingAnimator => _floatingAnimator;
        public Transform TrackPoint { get; set; }
        public Team Team { get; set; }
        public BoatUnitController Unit => _unit;
        public bool HasUnit => _unit != null;
        public bool IsMoving => _isMoving;
        public Action<BoatPart, Collider> TriggerHandler { get; set; }
        public IBoat HostBoat { get; set; } = null;
        public IDamageable Damageable { get; set; }
        public IArrowStuckTarget ArrowStuckTarget => _arrowStuckManager;
        public IDamagePointsProvider DamagePointsProvider => _damagePoints;
        public IBoatViewSettings ViewSettings { get; set; }
        public  int X { get; set; }
        public  int Y { get; set; }
        
        private void Awake()
        {
            foreach (var side in _sides)
                side.BoatPart = this;
            _arrowStuckManager = new ArrowStuckTarget(transform);
        }

        public void ColliderOff()
        {
            _trigger.enabled = false;
            _collider.enabled = false;
        }
        
        public void ColliderOn()
        {
            _trigger.enabled = true;
            _collider.enabled = true;
        }

        public void TriggerOff()
        {
            _trigger.enabled = false;
        }

        public void TriggerOnly()
        {
            _trigger.enabled = true;
            _collider.enabled = false;
        }

        public void SetView(IBoatViewSettings viewSettings)
        {
            ViewSettings = viewSettings;
            foreach (var side in _sides)
                side.View.SetView(viewSettings);
        }

        public void SetTempView(IBoatViewSettings viewSettings)
        {
            foreach (var side in _sides)
                side.View.SetView(viewSettings);
        }

        public void SetCurrentView()
        {
            foreach (var side in _sides)
                side.View.SetView(ViewSettings);
        }

        /// <summary>
        /// Moves to the provided local position as boat's child. Turns the colliders on complete
        /// </summary>
        public void MoveToConnectToBoat(Transform followPoint, Action onEnd)
        {
            var time = GlobalConfig.BoatPartConnectTime;
            StopAllMovement();
            _isMoving = true;
            var seq = DOTween.Sequence();
            _currentSequence = seq;
            var scaleSeq = DOTween.Sequence();
            scaleSeq.Append(transform.DOScale(Vector3.one * .8f, time * .5f));
            scaleSeq.Append(transform.DOScale(Vector3.one, time * .5f));
            // seq.Append(transform.DOLocalJump(followPoint.localPosition, jumpHeight, 1, time));
            seq.Append(transform.DOLocalMove(followPoint.localPosition, time).SetEase(Ease.InExpo));
            seq.Join(scaleSeq);
            seq.OnComplete(() =>
            {
                _isMoving = false;
                ColliderOn();
                onEnd?.Invoke();
            });
            
            RaiseOnAvailable(false);
        }

        public void BreakIntoPieces()
        {
            // CLog.Log($"[{gameObject.name}] broken");
            gameObject.SetActive(false);
            _arrowStuckManager.HideAll();
            StopAllMovement();
            ColliderOff();
            var broken = GCon.GOFactory.Spawn<BrokenBoatPart>(GlobalConfig.BoatPartBrokenID);
            broken.SetView(ViewSettings);
            broken.transform.CopyPosRot(transform);
            broken.Push();
            RaiseOnAvailable(false);
        }

        public void StopTweens()
        {
            DOTween.Kill(transform);
            _currentSequence.Kill();
        }

        public void StopAllMovement()
        {
            StopTweens();
            _isMoving = false;
        }
        
        public void StopFollowing()
        {
            StopTweens();
            _isMoving = false;
        }

        public void DropAndPush(Vector3 endPos, float time)
        {
            StopTweens();
            ColliderOff();
            _scalable.localScale = Vector3.one;
            _isMoving = true;
            const float jumpPower = 3f;
            transform.DOJump(endPos, jumpPower, 1, time).OnComplete(() =>
            {
                _isMoving = false;
                if (BoatUtils.CheckIfWalkable(endPos, this))
                {
                    TriggerOnly();
                    RaiseOnAvailable(true);
                }
                else
                {
                    BreakIntoPieces();
                }
            });
        }

        public void Hide()
        {
            _arrowStuckManager.HideAll();
            gameObject.SetActive(false);
            RaiseOnAvailable(false);
            ReturnToPool();
        }
        
        public void RaiseOnAvailable(bool available)
        {
            OnBecameAvailable?.Invoke(this, available);
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerHandler?.Invoke(this, other);
        }

#region Pooling
        public IObjectPool<BoatPart> Pool { get; set; }
        public BoatPart Target => this;

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
            if(Pool != null)
                Pool.ReturnObject(this);
        }

        public void HideForPool()
        {
            StopAllMovement();
            gameObject.SetActive(false);
            OnBecameAvailable = null;
            ResetUnitBack();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void ResetUnitBack()
        {
            if (Unit != null)
            {
                Unit.transform.parent = _scalable;
                Unit.transform.localPosition = Vector3.zero;
            }
        }
#endregion


        #region Editor
#if UNITY_EDITOR
        [ContextMenu("E_LogState")]
        public void E_LogState()
        {
            CLog.LogRed($"{gameObject.name} is moving {_isMoving}, has owner {HostBoat != null}, " +
                        $"collider on {_collider.enabled}, trigger on {_trigger.enabled}");
        }

        public void E_TopSideViewOnOff()
        {
            E_ChangeStateOfSide(0);
        }
        
        public void E_BotSideViewOnOff()
        {
            E_ChangeStateOfSide(2);
        }
        
        public void E_RightSideViewOnOff()
        {
            E_ChangeStateOfSide(1);
        }
        
        public void E_LeftSideViewOnOff()
        {
            E_ChangeStateOfSide(3);
        }

        private void E_ChangeStateOfSide(int index)
        {
            _sides[index].Awake();
            var on = _sides[index].View.IsSideOn;
            on = !on;
            if(on)
                _sides[index].View.ShowSideAndCorners();
            else
                _sides[index].View.HideSideAndCorners();
        }
#endif
        #endregion

    }
}