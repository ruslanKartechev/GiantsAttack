using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class BulletCasing : MonoBehaviour, IPooledObject<BulletCasing>
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;

        private const float Force = 10f;
        
        public void Drop(Transform fromPoint)
        {
            _rb.isKinematic = false;
            _collider.enabled = true;
            transform.position = fromPoint.position;
            gameObject.SetActive(true);
            _rb.AddForce(fromPoint.forward * Force, ForceMode.Impulse);
            _rb.AddTorque(fromPoint.up * Force, ForceMode.Impulse);
            Invoke(nameof(ReturnToPool), 3f);   
        }

        private void ReturnToPool()
        {
            Hide();
            Pool.ReturnObject(this);
        }
        

        public IObjectPool<BulletCasing> Pool { get; set; }
        public BulletCasing Obj => this;
        public void Parent(Transform parent)
        {
            transform.parent = parent;
        }

        public void Destroy()
        {
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}