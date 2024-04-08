using UnityEngine;

namespace GiantsAttack
{
    public abstract class SplineEvent : MonoBehaviour
    {
        // [SerializeField] protected float targetPercent;

        // public float Percent => targetPercent;
        
        public abstract void Activate();
    }
}