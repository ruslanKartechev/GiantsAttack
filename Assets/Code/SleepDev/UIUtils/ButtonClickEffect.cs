using UnityEngine;

namespace SleepDev.UIUtils
{
    public abstract class ButtonClickEffect : MonoBehaviour
    {
        [SerializeField] protected RectTransform _target;
        public abstract void Play();
    }
}