using System;
using GameCore.Cam;
using UnityEngine;

namespace GiantsAttack
{
    public abstract class LevelFinalSequence : MonoBehaviour
    {
        public IHelicopter Player { get; set; }
        public IMonster Enemy { get; set; }
        public PlayerCamera Camera { get; set; }
        
        public abstract void Begin(Action callback);
    }
}