using System;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;
using RaftsWar.Cam;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class CameraPointsManger
    {
        [SerializeField] private bool _notAllowedGoDown;
        [SerializeField] private Transform _parent;
        [SerializeField] private Transform _lookAt;
        private List<Transform> _points = new List<Transform>(10);
        private IPlayerCamera _camera;
        private int _index = -1;
        
        public IPlayerCamera Camera => _camera;

        public void InitCameraPoints(PlayerCameraPointsSettings settings)
        {
            _camera = BoatUtils.GetCamera();
            var points = settings.ConvertToPoints();
            // var rot = Quaternion.Euler(-settings.angle, 0 ,0 );
            foreach (var p in points)
            {
                var tr = new GameObject("cp").transform;
                // var wp = _parent.TransformPoint(p);
                tr.parent = _parent;
                tr.localScale = Vector3.one;
                tr.localPosition = p;
                tr.localRotation = Quaternion.LookRotation(_lookAt.localPosition - p);
                _points.Add(tr);
            }
        }
        
        public void StartCameraFollow()
        {
            _camera.AddCommand(new CameraCommandMoveToPointLocal(_parent, _points[0], GlobalConfig.PlayerCameraSetTime));
        }

        public void ResetIndex()
        {
            _index = -1;
        }
        
        public void SetCameraForCount(int count, float time = -1)
        {
            var index = CheckIndex(count);
            if (index == _index)
                return;
            if (index < _index && _notAllowedGoDown)
                return;
            _index = index;
            var point = _points[_index];
            if (time <= 0)
                time = GlobalConfig.PlayerCameraSetTime;
            _camera.AddCommand(new CameraCommandMoveToPointLocal(_parent, point, time));
        }

        private int CheckIndex(int count)
        {
            int index;
            if (count >= 4)
                index = 2;
            else if (count >= 2)
                index = 1;
            else
                index = 0;
            return index;
        }
     
        #if UNITY_EDITOR
        public void E_SetCameraToFirstPoint(PlayerCameraPointsSettings settings)
        {
            InitCameraPoints(settings);
            var cam = BoatUtils.GetCamera();
            cam.SetPoint(_points[0]);
            _camera = BoatUtils.GetCamera();
            var points = settings.ConvertToPoints();
            var p = _parent.TransformPoint(points[0]);
            var rot = Quaternion.LookRotation(_lookAt.position - p);
            var tr = new GameObject("tp").transform;
            tr.parent = _parent;
            tr.position = p;
            tr.rotation = rot;
            _camera.SetPosition(tr);
        }
        #endif
    }
}