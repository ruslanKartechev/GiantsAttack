using UnityEngine;

namespace GiantsAttack
{
    public class EnemyThrowableWeapon : MonoBehaviour, IEnemyThrowWeapon
    {
        [SerializeField] private SimpleThrowable _throwable;
        [SerializeField] private EnemyThrowWeaponHealth _health;
        
        public GameObject GameObject => gameObject;
        
        public IThrowable Throwable => _throwable;
        
        public ITarget Target => _health;
        
        public IHealth Health => _health;

    }

}