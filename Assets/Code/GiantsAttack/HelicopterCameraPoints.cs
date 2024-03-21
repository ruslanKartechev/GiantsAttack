using System;
using GameCore.Cam;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterCameraPoints : MonoBehaviour, IHelicopterCameraPoints
    {
        [SerializeField] private float _toInsideMoveTime = 1f;
        [SerializeField] private float _toOutsideMoveTime = 1f;
        [SerializeField] private Transform _insidePoint;
        [SerializeField] private Transform _outsidePoint;
        
        private IPlayerCamera _camera;
        
        public Transform InsidePoint => _insidePoint;
        public Transform OutsidePoint => _outsidePoint;
        
        
        public void SetCamera(IPlayerCamera camera)
        {
            _camera = camera;
        }
        
        public void MoveCameraToInside(Action callback)
        {
            _camera.MoveToPointToFollow(_insidePoint, _toInsideMoveTime, callback);
        }

        public void MoveCameraToOutside(Action callback)
        {
            _camera.MoveToPoint(_outsidePoint, _toOutsideMoveTime, callback);
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