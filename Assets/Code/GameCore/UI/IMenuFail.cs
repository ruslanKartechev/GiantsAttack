using System;

namespace GameCore.UI
{
    public interface IMenuFail : IUIScreen
    {
        void Show(Action buttonCallback, Action onDone);

    }
}