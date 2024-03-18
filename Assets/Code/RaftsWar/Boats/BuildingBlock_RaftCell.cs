using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BuildingBlock_RaftCell : MonoBehaviour, IBuildingBlock
    {
        [SerializeField] private BoatPart _raft;
        [SerializeField] private Vector3 _size;
        [SerializeField] private Transform _scalable;
        [SerializeField, Tooltip("IN CLOCKWISE DIRECTION")] private List<GameObject> _coreSides;

        public Vector3 CellSize => _size;
        
        public Transform Transform => transform;

        public void SetScale(float scale)
        {
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
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
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
                _raft.ColliderOff();
                return;
            }
#endif
            _raft.ColliderOff();
            _raft.ArrowStuckTarget.HideAll();
        }
        
        public void SetSide(ESquareSide side)
        {
            BoatUtils.SetSidesView(_raft, side);
            foreach (var s in _coreSides)
                s.SetActive(false);
            switch (side)
            {
                case ESquareSide.All:
                    foreach (var s in _coreSides)
                        s.SetActive(true);
                    break;
                case ESquareSide.None:
                    break;
                case ESquareSide.TopLeft:
                    _coreSides[0].SetActive(true);
                    _coreSides[3].SetActive(true);
                    break;
                case ESquareSide.Top:
                    _coreSides[0].SetActive(true);
                    break;
                case ESquareSide.TopRight:
                    _coreSides[0].SetActive(true);
                    _coreSides[1].SetActive(true);
                    break;
                case ESquareSide.Right:
                    _coreSides[1].SetActive(true);
                    break;
                case ESquareSide.BotRight:
                    _coreSides[2].SetActive(true);
                    _coreSides[1].SetActive(true);
                    break;
                case ESquareSide.Bot:
                    _coreSides[2].SetActive(true);
                    break;
                case ESquareSide.BotLeft:
                    _coreSides[2].SetActive(true);
                    _coreSides[3].SetActive(true);
                    break;
                case ESquareSide.Left:
                    _coreSides[3].SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
    }
}