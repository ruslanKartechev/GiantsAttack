using SleepDev;
using SleepDev.Ragdoll;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class UnitKilledEffect : MonoBehaviour
    {
        [SerializeField] private Ragdoll _ragdoll;
        [SerializeField] private Animator _animator;
        [SerializeField] private SinkingRagdoll _sinking;

        public void PlayKilled()
        {
            _animator.enabled = false;
            var force = Vector3.up - .5f * transform.forward;
            force *= GlobalConfig.UnitRagdollForce;
            _ragdoll.ActivateAndPush(force);
            _sinking.IsActive = true;
            _sinking.enabled = true;
        }
    }
}