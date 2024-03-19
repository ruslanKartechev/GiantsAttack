using RaftsWar.Cam;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterCameraPoints : MonoBehaviour, IHelicopterCameraPoints
    {
        [SerializeField] private float _toInsideMoveTime = 1f;
        [SerializeField] private Transform _insidePoint;
        [SerializeField] private Transform _outsidePoint;
        
        private IPlayerCamera _camera;
        
        public Transform InsidePoint => _insidePoint;
        public Transform OutsidePoint => _outsidePoint;
        
        
        public void SetCamera(IPlayerCamera camera)
        {
            _camera = camera;
        }
        
        public void MoveCameraToInside()
        {
            _camera.MoveToPoint(_insidePoint, _toInsideMoveTime, () => {});
        }
        
        public void SetCameraToOutside()
        {
            _camera.SetPoint(_outsidePoint);
        }
           
        public void SetCameraInside()
        {
            _camera.SetPoint(_insidePoint);
        }
    }
}