using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace SleepDev
{
    public class JoystickController : MonoBehaviour
    {
        public float Sensitivity { get; set; }
        
        protected float _maxRad;
        protected float _maxRad2;
        public float MaxRad
        {
            get => _maxRad;
            set
            {
                _maxRad = value;
                _maxRad2 = value * value;
            } 
        }
        protected Vector2 _position;
        protected Coroutine _working;
        
        public static JoystickController GetDefault()
        {
            var controller = new JoystickController();
            controller.Sensitivity = 1;
            controller.MaxRad = 100;
            return controller;
        }

        public void Reset()
        {
            _position = Vector2.zero;
        }

        public void Move(Vector2 delta)
        {
            _position += delta;
            var magn = _position.magnitude;
            if (magn > _maxRad)
                _position = _position / magn * _maxRad;
        }

        public float Percent()
        {
            return _position.magnitude / _maxRad;
        }

        public Vector3 GetXZPlane()
        {
            return new Vector3(_position.x, 0f, _position.y);
        }

        public Vector3 GetScaledXZPlane()
        {
            return new Vector3(_position.x, 0f, _position.y) / _maxRad;
        }

        public Vector3 GetPos()
        {
            return _position;
        }
        
        public void CheckDelta()
        {
            Stop();
            _working = StartCoroutine(CheckingDelta());
        }

        public virtual void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }
        
        protected IEnumerator CheckingDelta()
        {
            var pos = Input.mousePosition;
            while (true)
            {
                var nPos = Input.mousePosition;
                Vector2 delta = nPos - pos;
                delta *= Sensitivity;
                Move(delta);
                pos = nPos;
                yield return null;
            }
        }
    }
}