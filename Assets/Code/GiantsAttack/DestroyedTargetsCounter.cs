using System;
using GameCore.UI;
using SleepDev;

namespace GiantsAttack
{
    public class DestroyedTargetsCounter : IDestroyedTargetsCounter
    {
        private int _maxCount;
        private int _currentCount;
        
        public Action AllDestroyedCallback { get; set; }
        
        private CityDestroyUI _ui;
        
        public DestroyedTargetsCounter(CityDestroyUI ui, int maxCount, Action callback)
        {
            _ui = ui;
            AllDestroyedCallback = callback;
            SetCounter(maxCount);
        }
        
        public void SetCounter(int maxCount)
        {
            _maxCount = _currentCount = maxCount;
        }

        public void MinusOne(bool updateUI)
        {
            _currentCount--;
            CLog.Log($"[TargetsCounter] {_currentCount}/{_maxCount}");
            if (updateUI)
            {
                _ui.UpdateCount(_currentCount);
            }
            if (_currentCount <= 0)
            {
                AllDestroyedCallback.Invoke();
            }
        }

        public void Minus(int count, bool updateUI)
        {
            _currentCount -= count;
            CLog.Log($"[TargetsCounter] {_currentCount}/{_maxCount}");
            if (updateUI)
            {
                _ui.UpdateCount(_currentCount);
            }
            if (_currentCount <= 0)
            {
                AllDestroyedCallback.Invoke();
            }
        }
    }
}