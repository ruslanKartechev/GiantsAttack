using System;

namespace GameCore.UI
{
    public interface IMenuWin : IUIScreen
    {
        void Show(float addedTowerProgress, Action buttonCallback, Action onDone);
    }
}