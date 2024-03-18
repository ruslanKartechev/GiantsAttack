using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SleepDev
{
    public class ProperButton : Button
    {
        public event Action OnDown;
        public event Action OnUp;

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnDown?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnUp?.Invoke();

        }
    }
}