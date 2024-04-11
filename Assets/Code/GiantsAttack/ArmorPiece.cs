using UnityEngine;

namespace GiantsAttack
{
    public class ArmorPiece : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        
        public void PushAway(Vector3 center, float force)
        {
            transform.parent = null;
            _rb.isKinematic = false;
            _collider.enabled = true;
            var vec = (_rb.position - center).normalized;
            _rb.AddForce( vec* force, ForceMode.Impulse);
            var torque = Vector3.Cross(vec, Vector3.up);
            _rb.AddTorque( torque * (force * .5f), ForceMode.Impulse);
        }
    }
}