using System;

namespace GameCore.UI
{
    public interface IMenuStart : IUIScreen
    {
        void Show(Action playCallback, Action onShown, string objective);
        void ShowObjective(Action callback);
    }
}