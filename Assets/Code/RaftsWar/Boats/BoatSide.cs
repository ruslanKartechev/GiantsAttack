using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatSide : MonoBehaviour
    {
        [SerializeField] private Transform _direction;
        [SerializeField] private float _sideFaceAngle ;

        private IBoatSideView _view; 
        
        public bool IsOccupied { get; set; }
        public Transform Direction => _direction;
        public IBoatSideView View => _view;
        public float SideFaceAngle => _sideFaceAngle;
        public BoatPart BoatPart { get; set; }

        public void Awake()
        {
            _view = GetComponent<IBoatSideView>();
        }
    }
}