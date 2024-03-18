using UnityEngine;

namespace SleepDev.UIUtils
{
    public abstract class PopElement : MonoBehaviour
    {
        public abstract float Delay { get; set; }
        public abstract float Duration { get; set; }

        public abstract void ScaleUp();
    }
}