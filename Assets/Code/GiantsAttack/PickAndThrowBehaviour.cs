using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    /// <summary>
    /// Start executing on constructor call
    /// </summary>
    public class PickAndThrowBehaviour
    {
        private IThrowable _target;
        private IMonster _monster;
        private Transform _grabHand;
        private Action _callback;
        
        private static readonly int PickUp = Animator.StringToHash("PickUp");

        public PickAndThrowBehaviour(IThrowable target, IMonster monster, 
            Animator animator, Transform grabHand, 
            Action callback)
        {
            _target = target;
            _monster = monster;
            _callback = callback;
            _grabHand = grabHand;
            _monster.AnimEventReceiver.OnPickup += OnPickup;
            _monster.AnimEventReceiver.OnThrow += OnThrow;
            animator.SetTrigger(PickUp);
        }

        private void OnThrow()
        {
            CLog.Log($"[Grab&Throw] On Throw event");
            _monster.AnimEventReceiver.OnPickup -= OnPickup;
            _monster.AnimEventReceiver.OnThrow -= OnThrow;
            _callback.Invoke();
        }

        private void OnPickup()
        {
            _target.GrabBy(_grabHand, () =>
            {
                CLog.Log($"[Grab&Throw] On object grabbed");
            });
        }
    }
}