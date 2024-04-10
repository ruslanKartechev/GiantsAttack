using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class ChoppedMesh : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> _rbs;
        public List<Rigidbody> Rbs => _rbs;
    }
}