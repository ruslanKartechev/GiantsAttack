using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class StartSequenceSimple : LevelStartSequence
    {
        [SerializeField] private string _animationKey;
        [SerializeField] private List<StageListener> _listeners;

#if UNITY_EDITOR
        public override void E_Init()
        { }
#endif

        public override void Begin(Action onEnd)
        {
            if(_animationKey.Length > 0)
                Enemy.Animate(_animationKey, false);
            foreach (var listener in _listeners)
                listener.OnActivated();
            onEnd.Invoke();
        }
    }
}