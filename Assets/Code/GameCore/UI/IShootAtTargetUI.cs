using UnityEngine;

namespace GameCore.UI
{
    public interface IShootAtTargetUI
    {
        Transform CurrentTarget { get; }
        void ShowAndFollow(Transform target);
        void Hide();
    }
}