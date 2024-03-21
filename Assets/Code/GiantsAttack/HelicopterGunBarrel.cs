using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterGunBarrel : MonoBehaviour
    {
        [SerializeField] private Transform _fromPoint;
        [SerializeField] private Animator _animator;
        public Transform FromPoint => _fromPoint;

        public void Recoil()
        {
            _animator.Play("Recoil");
        }
    }
}