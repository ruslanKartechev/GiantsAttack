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
        public event Action OnAnimationOver;
        public event Action OnJumpDown;
        public event Action OnStoodUp;

        public void EventOnStoodUp()
        {
            OnStoodUp?.Invoke();
        }        

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

        public void EventOnJumpDown()
        {
            OnJumpDown?.Invoke();   
        }
    
        public void EventOnAnimationOver()
        {
            OnAnimationOver?.Invoke();   
        }
    }
    
}