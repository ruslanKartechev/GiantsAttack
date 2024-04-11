using System;
using UnityEngine;

namespace GiantsAttack
{
    public class EnemyThrowableGroundObject : MonoBehaviour, IEnemyThrowWeapon
    {
        [SerializeField] private SimpleThrowable _throwable;
        [SerializeField] private EnemyThrowWeaponHealth _health;
        [SerializeField] private AnimatedVehicleBase _animatedVehicle;

        public GameObject GameObject => gameObject;
        public IThrowable Throwable => _throwable;
        public IHealth Health => _health;
        public AnimatedVehicleBase AnimatedVehicle => _animatedVehicle;
    }

}