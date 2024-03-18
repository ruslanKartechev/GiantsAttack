using DG.Tweening;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BuildingBlock_Raft : MonoBehaviour, IBuildingBlock
    {
        [SerializeField] private BoatPart _raft;
        [SerializeField] private Vector3 _size;
        [SerializeField] private Transform _scalable;
        [SerializeField] private FloatingAnimator _floatingAnimator;

        public Vector3 CellSize => _size;
        public Transform Transform => transform;

        public void SetScale(float scale)
        {
            _scalable.DOKill();
            _scalable.localScale = Vector3.one * scale;
        }

        public void HideRampSide(ESquareSide side)
        {
            BoatUtils.HideRampSidesView(_raft, side);
        }

        /// <summary>
        /// Should be used after SCALE, because targets the same Transform
        /// </summary>
        public void SetYScale(float yScale)
        {
            _scalable.SetYScale(yScale);
        }

        public void Destroy()
        {
            _raft.BreakIntoPieces();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Take()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                _raft.TriggerOff();
                return;
            }
#endif
            _raft.StopAllMovement();
            _raft.transform.localScale = Vector3.one;

            _floatingAnimator.enabled = false;
            _raft.ArrowStuckTarget.HideAll();
            _raft.enabled = false;
            _raft.TriggerOff();
            _raft.RaiseOnAvailable(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void SetSide(ESquareSide side)
        {
            BoatUtils.SetSidesView(_raft, side);
        }
    }
}