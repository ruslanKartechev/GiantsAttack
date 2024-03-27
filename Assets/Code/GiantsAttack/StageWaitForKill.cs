using UnityEngine;

namespace GiantsAttack
{
    public class StageWaitForKill : LevelStage
    {
        [SerializeField] private bool _callRoar;
        
        public override void Activate()
        {
            Player.Aimer.BeginAim();
            if(_callRoar)
                Enemy.Roar();
            SubToEnemyKill();
        }

        public override void Stop()
        { }
        
     
    }
}