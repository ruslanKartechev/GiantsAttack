using UnityEngine;

namespace GameCore.UI
{
    public interface IAimUI
    {
        void Show(bool animated);
        void Hide(bool animated);
        void SetPosition(Vector3 screenPos);
    }
}