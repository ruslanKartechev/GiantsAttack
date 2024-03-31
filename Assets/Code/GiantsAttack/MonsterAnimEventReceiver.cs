using System;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterAnimEventReceiver : MonoBehaviour, IMonsterAnimEventReceiver
    {
        public event Action OnPickup;
        public event Action OnThrow;
        public event Action OnPunch;
        public event Action OnPunchBegan;

        public void EventOnPunch()
        {
            OnPunch?.Invoke();
        }    
        
        public void EventOnPunchBegan()
        {
            OnPunchBegan?.Invoke();
        }        
        
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