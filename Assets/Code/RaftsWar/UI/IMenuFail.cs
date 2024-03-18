using System;

namespace RaftsWar.UI
{
    public interface IMenuFail : IUIScreen
    {
        void Show(Action buttonCallback, Action onDone);

    }
}