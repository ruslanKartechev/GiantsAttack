using System;

namespace GameCore.UI
{
    public interface IMenuFail : IUIScreen
    {
        void Show(int level, Action buttonCallback, Action onDone);

    }
}