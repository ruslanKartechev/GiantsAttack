using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class PlayerMover : MonoBehaviour, IPlayerMover
    {
        [SerializeField] private List<PathNode> _nodes;
        private PathNode _currentNode;
        private int _nodeIndex;
        
        
        
        [System.Serializable]
        public class PathNode
        {
            public Transform point;
            public float moveTime;
            public AnimationCurve curve;
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
            
        }

        public void Evade(EDirection2D direction2D, Action callback, float distance)
        {
            Player.Mover.Evade(direction2D, callback, distance);
        }

        public void Begin()
        {
            _currentNode = _nodes[0];
            // Player.Mover.MoveTo();
        }
        
        
        
        
        
        
        
        #if UNITY_EDITOR
        [Header("Gizmos"), Space(10)] 
        public bool e_drawGizmos;
        
        public void OnDrawGizmos()
        {
            if (!e_drawGizmos)
                return;
            if (_nodes.Count < 2)
                return;
            var oldColor = Gizmos.color;
            Gizmos.color = Color.black;
            var cubeSize = 1f;
            for (var i = 1; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                if(node.point == null)
                    continue;
                var prev = _nodes[i - 1].point.position;
                var current = _nodes[i].point.position;
                Gizmos.DrawLine(prev, current);
                Gizmos.DrawCube(_nodes[i-1].point.position, Vector3.one * cubeSize);
                Gizmos.DrawCube(_nodes[i].point.position, Vector3.one * cubeSize);
            }
            
            Gizmos.color = oldColor;
        }
#endif
    }
}