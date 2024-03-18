using System;

namespace RaftsWar.UI
{
    public interface IMenuStart : IUIScreen
    {
        void Show(Action playCallback, Action onShown);
    }
}