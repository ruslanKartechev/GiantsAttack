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
        
        public IGameplayMenu UI { get; set; }
        private ITargetsCountUI _countUI;
        
        public DestroyedTargetsCounter(IGameplayMenu ui, int maxCount, Action callback)
        {
            UI = ui;
            AllDestroyedCallback = callback;
            SetCounter(maxCount);
            _countUI = UI.TargetsCountUI;
            _countUI.SetCount(_currentCount, maxCount);
            _countUI.Show(true);
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
                _countUI.UpdateCount(_currentCount);
            }
            if (_currentCount == 0)
            {
                AllDestroyedCallback.Invoke();
            }
        }
    }
}