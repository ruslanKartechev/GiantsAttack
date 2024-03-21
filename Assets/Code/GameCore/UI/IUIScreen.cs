using System;
using UnityEngine;

namespace GameCore.UI
{
    public interface IUIScreen
    {
        void On();
        void Off();
        void Show(Action onDone);
        void Hide(Action onDone);
        GameObject Go { get; }
    }
}