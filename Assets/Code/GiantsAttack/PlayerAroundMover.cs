using System;
using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class PlayerAroundMover : MonoBehaviour, IPlayerMover
    {
        [SerializeField] private float _startDelay;
        [SerializeField] private Transform _center;
        [SerializeField] private Transform _lookAt;
        [SerializeField] private Transform _orientation;
        [SerializeField] private List<HelicopterMoveAroundNode> _nodes;

        public IHelicopter Player { get; set; }

        public void Pause(bool loiter)
        {
        }

        public void Resume()
        {
        }

        public void Evade(EDirection2D direction2D, Action callback, float distance)
        {
        }

        public void Begin()
        {
            StartCoroutine(Working());
        }


        private IEnumerator Working()
        {
            yield return new WaitForSeconds(_startDelay);
            if (_orientation == null)
            {
                var orientation = new GameObject("orientation").transform;
                orientation.SetPositionAndRotation(_center.position, _center.rotation);
                _orientation = orientation;
            }
            Player.Mover.BeginMovingAround(new HelicopterMoveAroundData(_center, _lookAt, _orientation));
            foreach (var node in _nodes)
            {
                yield return new WaitForSeconds(node.startDelay);
                Player.Mover.ChangeMovingAroundNode(node);
            }
        }
        
    }
}