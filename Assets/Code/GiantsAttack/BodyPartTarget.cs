using UnityEngine;

namespace GiantsAttack
{
    public class BodyPartTarget : MonoBehaviour, ITarget
    {
        public IDamageable Damageable { get; set; }
    }
}