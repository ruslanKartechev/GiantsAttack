using System;
using UnityEngine;

namespace GiantsAttack
{
    public class SoftBall : MonoBehaviour
    {
        public Rigidbody rb;
        
        public Action<Collider> CollisionCallback;

        private void OnCollisionEnter(Collision collision)
        {
            CollisionCallback.Invoke(collision.collider);
        }
    }
}