using System;
using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class PlayerMover : MonoBehaviour, IPlayerMover
    {
        [SerializeField] private List<PathNode> _nodes;
        [SerializeField] private Transform _startPoint;
        private PathNode _currentNode;
        private int _nodeIndex;
        private float _elapsedAwaiting;
        private bool _isAwaiting;
        private bool _isWaiting;
        private Coroutine _waiting;


        [System.Serializable]
        public class PathNode
        {
            public Transform point;
            public float moveTime;
            public AnimationCurve curve;
            public float startDelay;
        }

        public IHelicopter Player { get; set; }

        public void Pause(bool loiter)
        {
            Player.Mover.StopAll();
            if(loiter)
                Player.Mover.Loiter();
        }

        public void Resume()
        {
            CLog.Log($"[PlayerMover] Resume, waiting: {_isWaiting}, index = {_nodeIndex}");
            if (_isWaiting)
            {
                CLog.Log($"[PlayerMover] Resume, is still waiting");
                WaitForNode();
                return;
            }
            if (Player.Mover.ResumeMovement())
                return;
            MoveToNode(_nodes[_nodeIndex]);
        }

        public void Evade(EDirection2D direction2D, Action callback, float distance)
        {
            CLog.Log($"[PlayerMover] Evade direction {direction2D.ToString()}");
            StopWaiting();
            Player.Mover.Evade(direction2D, callback, distance);
        }

        public void Begin()
        {
            _nodeIndex = 0;
            _elapsedAwaiting = 0f;
            _currentNode = _nodes[_nodeIndex];
            WaitForNode();
            // MoveToNode(_currentNode);
        }

        private void MoveToNode(PathNode node)
        {
            _currentNode = node;
            var data = new HelicopterMoveToData(node.point, node.moveTime, node.curve, OnMovedToNode);
            Player.Mover.MoveTo(data);
        }

        private void OnMovedToNode()
        {
            CLog.Log($"[PlayerMover] On moved to node index {_nodeIndex}");
            _nodeIndex++;
            if (_nodeIndex >= _nodes.Count)
            {
                CLog.LogRed("ALL NODES PASSED");
                return;
            }
            _elapsedAwaiting = 0f;
            WaitForNode();
            // var node = _nodes[_nodeIndex];
            // MoveToNode(node);
        }


        private void StopWaiting()
        {
            if(_waiting != null)
                StopCoroutine(_waiting);
        }

        private void WaitForNode()
        {
            StopWaiting();
            _waiting = StartCoroutine(WaitingToStartNode());
        }
        
        private IEnumerator WaitingToStartNode()
        {
            _isWaiting = true;
            var time = _nodes[_nodeIndex].startDelay;
            while (_elapsedAwaiting < time)
            {
                _elapsedAwaiting += Time.deltaTime;
                yield return null;
            }
            _isWaiting = false;
            _elapsedAwaiting = 0f;
            MoveToNode(_nodes[_nodeIndex]);
        }
        
        
        
#if UNITY_EDITOR
        [Header("Gizmos"), Space(10)] 
        public bool e_drawGizmos;
        
        public void OnDrawGizmos()
        {
            if (!e_drawGizmos)
                return;
            if (_nodes.Count < 2 || _startPoint == null)
                return;
            var oldColor = Gizmos.color;
            Gizmos.color = Color.black;
            var cubeSize = 1f;
            var prevPoint = _startPoint;
            for (var i = 0; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                if(node.point == null)
                    continue;
                var prev = prevPoint.position;
                var current = _nodes[i].point.position;
                Gizmos.DrawLine(prev, current);
                Gizmos.DrawCube(prevPoint.position, Vector3.one * cubeSize);
                Gizmos.DrawCube(_nodes[i].point.position, Vector3.one * cubeSize);
                prevPoint = _nodes[i].point;
            }
            
            Gizmos.color = oldColor;
        }
#endif
    }
}