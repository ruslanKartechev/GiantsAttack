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
        private Action _throwCallback;
        private Action _pickCallback;

        private static readonly int PickUp = Animator.StringToHash("PickUp");

        public PickAndThrowBehaviour(IThrowable target, IMonster monster, 
            Animator animator, Transform grabHand, 
            Action onPickCallback, Action throwCallback)
        {
            _target = target;
            _monster = monster;
            _throwCallback = throwCallback;
            _pickCallback = onPickCallback;
            
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
            _throwCallback.Invoke();
        }

        private void OnPickup()
        {
            _pickCallback?.Invoke();
            _target.GrabBy(_grabHand, () =>
            {
                CLog.Log($"[Grab&Throw] On object grabbed");
            });
        }
    }
}