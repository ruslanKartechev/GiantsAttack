using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    [System.Serializable]
    public class CorrectSwipeChecker
    {
        [SerializeField] private EDirection2D _correctDirection;
        [SerializeField] private SwipeInputTaker _swipeInputTaker;
        [SerializeField] private float _targetSwipeDistance = 50f;

        public EDirection2D CorrectDirection => _correctDirection;
        public EDirection2D LastSwipeDir { get; private set; }
        public Action OnCorrect { get; set; }
        public Action OnWrong { get; set; }      
        
        public void On()
        {
            _swipeInputTaker.enabled = true;
            _swipeInputTaker.TargetDistance = _targetSwipeDistance;
            _swipeInputTaker.OnSwipeIndirection -= OnSwiped;
            _swipeInputTaker.OnSwipeIndirection += OnSwiped;
        }

        public void Off()
        {
            _swipeInputTaker.enabled = false;
            _swipeInputTaker.OnSwipeIndirection -= OnSwiped;
        }
        
        private void OnSwiped(EDirection2D dir)
        {
            LastSwipeDir = dir;
            if(dir == _correctDirection)
                OnCorrect.Invoke();
            else
                OnWrong.Invoke();
        }
    }
}