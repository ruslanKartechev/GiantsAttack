using UnityEngine;

namespace GiantsAttack
{
    public class RunawayCar : MonoBehaviour, IRunaway
    {
        [SerializeField] private SplineMover _mover;
        [SerializeField] private GameObject _go;
        
        public SplineMover Mover => _mover;

        public void Init()
        {
            _go.SetActive(true);
        }

        public void BeginMoving()
        {
        }

        public void Stop()
        {
        }

        public void Kill()
        {
        }

    }
}