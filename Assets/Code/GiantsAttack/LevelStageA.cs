using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    /// <summary>
    /// Stage A - player is flying around the monster and shooting
    /// </summary>
    public class LevelStageA : LevelStage
    {
        [SerializeField] private CircularPathBuilder _circularPathBuilder;
        [SerializeField] private Transform _lookAtTarget;
        
        public override void Activate()
        {
            Player.Aimer.BeginAim();
            Player.Mover.BeginAnimating();
            Player.Mover.BeginMovingOnCircle(_circularPathBuilder.Path, _lookAtTarget, false, OnDoneMoving);
            Enemy.Roar();
        }

        public override void Stop()
        {
            
        }

        private void OnDoneMoving()
        {
            CLog.Log($"[LevelStageA] completed");
            CompletedCallback?.Invoke();
        }
    }
}