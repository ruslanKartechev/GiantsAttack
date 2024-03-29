using SleepDev.Ragdoll;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterDestroyer : MonoBehaviour, IDestroyer
    {
        [SerializeField] private float _pushForce;
        [SerializeField] private float _pushForceUp;
        
        [SerializeField] private string _killTriggerName;
        [SerializeField] private Animator _animator;
        [SerializeField] private Ragdoll _ragdoll;
        
        public void DestroyMe()
        {
            _animator.enabled = false;
            _ragdoll.ActivateAndPush(-transform.forward * _pushForce + Vector3.up * _pushForceUp);
            // _animator.SetTrigger(_killTriggerName);   
        }
    }
}