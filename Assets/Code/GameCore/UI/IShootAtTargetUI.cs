using UnityEngine;

namespace GameCore.UI
{
    public interface IShootAtTargetUI
    {
        void ShowAndFollow(Transform target);
        void Hide();
    }
}