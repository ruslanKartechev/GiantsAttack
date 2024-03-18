using System;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CatapultAnimator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        public Action Callback;
        private static readonly int Idle1 = Animator.StringToHash("Idle");
        private static readonly int Shoot1 = Animator.StringToHash("Shoot");

        public void SetShootSpeed(float speed)
        {
            _animator.SetFloat("Speed", speed);
        }
        
        public void Idle()
        {
            _animator.ResetTrigger(Shoot1);
            _animator.SetTrigger(Idle1);
        }
        
        public void Shoot()
        {
            _animator.ResetTrigger(Idle1);
            _animator.SetTrigger(Shoot1);
        }
        
        public void OnShootAnim()
        {
            Callback.Invoke();
        }
    }
}