using System;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterAnimEventReceiver : MonoBehaviour, IMonsterAnimEventReceiver
    {
        public event Action OnPickup;
        public event Action OnThrow;

        public void EventOnPickup()
        {
            OnPickup?.Invoke();
        }

        public void EventOnThrow()
        {
            OnThrow?.Invoke();
        }
    }
}