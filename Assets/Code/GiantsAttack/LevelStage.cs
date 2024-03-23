using System;
using GameCore.Cam;
using UnityEngine;

namespace GiantsAttack
{
    public abstract class LevelStage : MonoBehaviour
    {
        
        public virtual IHelicopter Player { get; set; }
        public virtual IMonster Enemy { get; set; }
        public virtual PlayerCamera Camera { get; set; }
        public virtual Action CompletedCallback { get; set; }
        
        public abstract void Activate();
        public abstract void Stop();
    }
}