using System;
using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class StartSequenceSimple : LevelStartSequence
    {
        [SerializeField] private bool _waitForAnimationEnd;
        [SerializeField] private string _animationKey;
        [SerializeField] private List<StageListener> _listeners;
        private Action _callback;
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
            if (_waitForAnimationEnd)
            {
                _callback = onEnd;
                Enemy.AnimEventReceiver.EOnAnimationOver += Callback;
            }   
            else
            {
                onEnd.Invoke();
            }
        }

        private void Callback()
        {
            Enemy.AnimEventReceiver.EOnAnimationOver -= Callback;
            _callback.Invoke();
        }
    }
}