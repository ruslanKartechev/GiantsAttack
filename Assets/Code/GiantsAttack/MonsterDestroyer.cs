using SleepDev.Ragdoll;
using UnityEngine;

namespace GiantsAttack
{
    public class MonsterDestroyer : MonoBehaviour, IMonsterDestroyed
    {
        [SerializeField] private float _pushForce;
        [SerializeField] private float _pushForceUp;
        [SerializeField] private float _choppedForce;
        [SerializeField] private Animator _animator;
        [SerializeField] private Ragdoll _ragdoll;
        [SerializeField] private ChoppedMeshSpawner _choppedMeshSpawner;
        
        public void DestroyMe()
        {
            _animator.enabled = false;
            _ragdoll.ActivateAndPush(-_animator.transform.forward * _pushForce + Vector3.up * _pushForceUp);
        }

        public void DestroyMeChopped()
        {
            _choppedMeshSpawner.PlayWithForce(_choppedForce);
        }
    }
}