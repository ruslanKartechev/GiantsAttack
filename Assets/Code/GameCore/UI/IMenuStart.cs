using System;

namespace GameCore.UI
{
    public interface IMenuStart : IUIScreen
    {
        void Show(Action playCallback, Action onShown);
    }
}