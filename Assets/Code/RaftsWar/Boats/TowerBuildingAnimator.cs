using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerBuildingAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject _go;
        [SerializeField] private float _upOffet = 2f;
        
        public void ShowAt(ITowerBlocksBuilder builder)
        {
            _go.SetActive(true);
            var pos = builder.LatestGrid.WorldSquare.Center;
            pos.y = _upOffet;
            _go.transform.position = pos;
        }

        public void Hide()
        {
            _go.SetActive(false);
        }
    }
}