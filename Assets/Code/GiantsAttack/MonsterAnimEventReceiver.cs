using System;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterAnimEventReceiver : MonoBehaviour, IMonsterAnimEventReceiver
    {
        public event Action EOnPickup; // OnPickup
        public event Action EOnThrow; // OnThrow
        public event Action EOnPunch; // OnPunch
        public event Action EOnPunchBegan; // OnPunchBegan
        public event Action EOnAnimationOver; // OnAnimationOver
        public event Action EOnJumpDown; // OnJumpDown
        public event Action EOnStoodUp; // OnStoodUp

        public void OnStoodUp()
        {
            EOnStoodUp?.Invoke();
        }        

        public void OnPunch()
        {
            EOnPunch?.Invoke();
        }    
        
        public void OnPunchBegan()
        {
            EOnPunchBegan?.Invoke();
        }        
        
        public void OnPickup()
        {
            EOnPickup?.Invoke();
        }

        public void OnThrow()
        {
            EOnThrow?.Invoke();
        }

        public void OnJumpDown()
        {
            EOnJumpDown?.Invoke();   
        }
    
        public void OnAnimationOver()
        {
            EOnAnimationOver?.Invoke();   
        }
    }
    
}