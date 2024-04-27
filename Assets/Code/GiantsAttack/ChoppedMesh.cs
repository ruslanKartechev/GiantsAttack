using System.Collections.Generic;
using UnityEngine;

namespace GiantsAttack
{
    public class ChoppedMesh : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> _rbs;
        [SerializeField] private List<MeshRenderer> _renderers;
        
        public List<Rigidbody> Rbs => _rbs;

        public void SetView(EnemyView view)
        {
            foreach (var rend in _renderers)
            {
                view.SetView(rend);
            }
        }
    }
}