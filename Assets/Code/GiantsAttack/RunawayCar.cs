using UnityEngine;

namespace GiantsAttack
{
    public class RunawayCar : MonoBehaviour, IRunaway
    {
        [SerializeField] private SplineMover _mover;
        [SerializeField] private GameObject _go;
        [SerializeField] private Animator _animator;
        
        public SplineMover Mover => _mover;

        public void Init()
        {
            _go.SetActive(true);
        }

        public void BeginMoving()
        {
            _mover.MoveNow();
        }

        public void ChangeSpeed(float finalSpeed, float changeDuration)
        {
            _mover.ChangeSpeed(finalSpeed, changeDuration);
        }

        public void Stop()
        {
            
        }

        public void Kill()
        {
            
        }

    }
}