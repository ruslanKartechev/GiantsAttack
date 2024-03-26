using UnityEngine;

namespace GiantsAttack
{
    public class EnemyThrowableDestroyer : MonoBehaviour, IDestroyer
    {
        [SerializeField] private ParticleSystem _particles;
        
        public void DestroyMe()
        {
            gameObject.SetActive(false);
            _particles.transform.parent = transform.parent;
            _particles.Play();
        }
    }
}