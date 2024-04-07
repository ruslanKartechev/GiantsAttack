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
        }

        private void MoveToNode(PathNode node)
        {
            _currentNode = node;
            var data = new HelicopterMoveToData(node.point, node.moveTime, node.curve, OnMovedToNode);
            Player.Mover.MoveTo(data);
        }

        private void OnMovedToNode()
        {
            CLog.Log($"[{nameof(PlayerMover)}] On moved to node index {_nodeIndex}");
            _nodeIndex++;
            if (_nodeIndex >= _nodes.Count)
            {
                CLog.LogRed($"[{nameof(PlayerMover)}] All nodes passed");
                return;
            }
            _elapsedAwaiting = 0f;
            WaitForNode();
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
        public float e_moveSpeed;
        public Color e_color = Color.black;
        
        public void OnDrawGizmos()
        {
            if (!e_drawGizmos)
                return;
            if (_nodes.Count < 2 || _startPoint == null)
                return;
            var oldColor = Gizmos.color;
            Gizmos.color = e_color;
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

        [ContextMenu("E_CalculateTimeFromSpeed")]
        public void E_CalculateTimeFromSpeed()
        {
            if (_nodes.Count == 0)
                return;
            if (_startPoint != null)
            {
                _nodes[0].moveTime = (_nodes[0].point.position - _startPoint.position).magnitude / e_moveSpeed;
                UnityEditor.EditorUtility.SetDirty(this);
            }

            if (_nodes.Count < 2)
                return;
            for (var i = 1; i < _nodes.Count; i++)
            {
                var prev = _nodes[i - 1].point;
                var curr = _nodes[i].point;
                var distance = (curr.position - prev.position).magnitude;
                var time = distance / e_moveSpeed;
                _nodes[i].moveTime = time;
            }
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}