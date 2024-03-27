using UnityEngine;

namespace GiantsAttack
{
    public class MonsterDestroyer : MonoBehaviour, IDestroyer
    {
        [SerializeField] private string _killTriggerName;
        [SerializeField] private Animator _animator;
        
        public void DestroyMe()
        {
            _animator.SetTrigger(_killTriggerName);   
        }
    }
}