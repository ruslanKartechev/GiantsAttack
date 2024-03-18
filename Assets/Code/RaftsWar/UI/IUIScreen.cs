using System;
using UnityEngine;

namespace RaftsWar.UI
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