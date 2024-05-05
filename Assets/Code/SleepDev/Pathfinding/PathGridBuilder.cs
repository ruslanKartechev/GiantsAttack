using UnityEngine;

namespace SleepDev
{
    public class PathGridBuilder : MonoBehaviour
    {
        [SerializeField] private bool _doDraw;
        [SerializeField] private float _size;
        [SerializeField] private PathGrid _grid;

        private void OnDrawGizmos()
        {
            if (_doDraw)
            {
                
            }
        }
        
        
    }
}

