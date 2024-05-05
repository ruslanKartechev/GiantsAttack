using System;

namespace GameCore.UI
{
    public interface IMenuWin : IUIScreen
    {
        void Show(int level, Action buttonCallback, Action onDone);
        TextByCharPrinter ResultPrinter { get; }
        LevelResultsUI LevelResultsUI { get; }
    }
}