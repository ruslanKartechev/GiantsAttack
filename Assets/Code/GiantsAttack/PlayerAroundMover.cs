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
        private Coroutine _working;
        
        public IHelicopter Player { get; set; }
        public IMonster Enemy { get; set; }

        public void Resume()
        { }

        public void Evade(EDirection2D direction2D, Action callback, float distance)
        { }
        
        public void Pause(bool loiter)
        {
            if(_working != null)
                StopCoroutine(_working);
            Player.Mover.StopAll();
        }

        public void Begin()
        {
            if(_working != null)
                StopCoroutine(_working);
            _working = StartCoroutine(Working());
        }

        public void SkipToNextPoint()
        {
            throw new NotImplementedException();
        }

        public void ZeroWaitTime()
        {
            throw new NotImplementedException();
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
            for (var i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                yield return new WaitForSeconds(node.startDelay);
                #if UNITY_EDITOR
                LogNode(i);
                #endif
                Player.Mover.ChangeMovingAroundNode(node);
            }
#if UNITY_EDITOR
            CLog.LogRed($"All Nodes passed");
#endif
        }

        private void LogNode(int index)
        {
            var node = _nodes[index];
            var msg = $"[PAM] Node {index}, ";
            if (node.changeAngle)
            {
                msg += $"Ang {node.angle}, {node.timeToChangeAngle}s, ";
            }
            if (node.changeHeight)
            {
                msg += $"H {node.height}, {node.timeToChangeHeight}s, ";
            }
            if (node.changeRadius)
            {
                msg += $"Rad {node.radius}, {node.timeToChangeRadius}s";
            }
            CLog.Log(msg);
        }

    }
}