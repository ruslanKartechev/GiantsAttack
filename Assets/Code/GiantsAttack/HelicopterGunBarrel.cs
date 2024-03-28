using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterGunBarrel : MonoBehaviour
    {
        [SerializeField] private Transform _fromPoint;
        [SerializeField] private Transform _dropPoint;
        [SerializeField] private Animator _animator;
        [SerializeField] private ZRotator _rotator;
        
        public Transform FromPoint => _fromPoint;
        public Transform DropPoint => _dropPoint;

        public void Rotate(bool on)
        {
            _rotator.enabled = on;
        }

        public void Recoil()
        {
            _animator.Play("Recoil");
        }
    }
}