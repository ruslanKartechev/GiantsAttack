using System;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class StageMovementExecutor
    {
        [Header("Await type")]
        [SerializeField] protected EWaitForMovement _waitType;
        [Header("Player")] 
        [SerializeField] protected bool _moveAfterEvaded;
        [SerializeField] protected float _afterEvasionMoveTime;
        [SerializeField] protected Transform _afterEvasionMovePoint;
        [Header("Enemy")] 
        [SerializeField] protected bool _enemyMoveAfterEvaded;
        [SerializeField] protected float _enemyAfterEvasionMoveTime;
        [SerializeField] protected Transform _enemyAfterEvasionMovePoint;
        private bool _completed;

        public Action FinalCallback { get; set; }
        
        public void MovePlayer(Action callback)
        {
            
        }

        public void MoveEnemy(Action callback)
        {
            
        }
    }
}