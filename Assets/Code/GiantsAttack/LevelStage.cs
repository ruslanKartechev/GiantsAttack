using System;
using GameCore.Cam;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public abstract class LevelStage : MonoExtended
    {
        public virtual IGameplayMenu UI { get; set; }
        public virtual IHelicopter Player { get; set; }
        public virtual IMonster Enemy { get; set; }
        public virtual PlayerCamera Camera { get; set; }
        public virtual Action CompletedCallback { get; set; }
        
        public abstract void Activate();
        public abstract void Stop();

        protected virtual void DestroyPlayerAndFail()
        {
            CLog.LogRed($"FAILED LEVEL STAGE");
        }
    }
}