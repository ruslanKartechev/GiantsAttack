using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RaftsWar.Boats
{
    public class EnemyNavAgent : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agentPreefab;
        [SerializeField] private Boat _boat;
        [SerializeField] private List<float> _radii;
        [SerializeField] private List<Vector3> _localPositions;
        private NavMeshAgent _agent;
        public float Radius => _agent.radius;
        public NavMeshPath CurrentPath => _agent.path;
        public Vector3 Position => _agent.nextPosition;

        private void Spawn()
        {
            _agent = Instantiate(_agentPreefab, transform.parent);
            _agent.transform.SetPositionAndRotation(transform.position, transform.rotation);
            _agent.avoidancePriority = UnityEngine.Random.Range(1, 99);
        }

        public void On()
        {
            if(_agent == null)
                Spawn();
            _agent.enabled = true;
            _agent.updateRotation = false;
            _agent.updatePosition = false;
       
        }

        public void Off()
        {
            _agent.enabled = false;
        }

        public void AdjustRadius()
        {
            // _agent.enabled = false;
            if (_boat.Parts.Count == 0)
            {
                // _center.position = _boat.RootPart.Point.position;
                _agent.radius = _radii[0];
            }
            else
            {
                var ind = _boat.Parts.Count;
                if(ind >= _localPositions.Count)
                    ind = _localPositions.Count-1;
                // _center.localPosition = _localPositions[ind];
                _agent.radius = _radii[ind];
            }
            // _agent.enabled = true;
        }

        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public void WarpTo(Vector3 position)
        {
            _agent.Warp(position);
        }
        
        public bool MoveTo(Vector3 endPoint)
        {
            // _agent.ResetPath();
            _agent.isStopped = false;
            return _agent.SetDestination(endPoint);
        }

        public bool CalculatePath(Vector3 endPoint, NavMeshPath path)
        {
            return _agent.CalculatePath(endPoint, path);
        }

        public float RemainingDistance()
        {
            return _agent.remainingDistance;
        }

        public Vector3 CurrentDestination() => _agent.destination;
        
        public void Stop()
        {
            _agent.ResetPath();
            // _agent.isStopped = true;
        }
    }
}