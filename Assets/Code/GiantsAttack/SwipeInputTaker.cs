using System;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SwipeInputTaker : MonoBehaviour
    {
        private Vector3 _pointerPos;
        private float _magnitude;
        private bool _isDown;    

        public EDirection2D Direction { get; private set; }
        public float TargetDistance { get; set; } = 50f;
        public float Distance { get; private set; }
        
        
        public event Action<EDirection2D> OnSwipeIndirection;

        public void Refresh()
        {
            Distance = 0f;
            _isDown = false;
            _pointerPos = Input.mousePosition;
        }

        private void OnEnable()
        {
            _pointerPos = Input.mousePosition;
            _isDown = false;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDown = true;
                _pointerPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _isDown = false;
                Distance = 0f;
                _pointerPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0) && _isDown)
            {
                var pos = Input.mousePosition;
                var delta = pos - _pointerPos;
                var direction = EDirection2D.Up;
                var dm = 0f;
                if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
                {
                    if (delta.x > 0)
                    {
                        dm += delta.x;
                        direction = EDirection2D.Right;
                    }
                    else
                    {
                        dm += Mathf.Abs(delta.x);
                        direction = EDirection2D.Left;
                    }
                }
                else
                {
                    if (delta.y > 0)
                    {
                        dm += delta.y;
                        direction = EDirection2D.Up;
                    }
                    else
                    {
                        dm += Mathf.Abs(delta.y);
                        direction = EDirection2D.Down;
                    }
                }

                if (direction == Direction)
                {
                    Distance += dm;
                }
                else
                {
                    Direction = direction;
                    Distance = dm;
                }
                _pointerPos = pos;
                if (Distance >= TargetDistance)
                {
                    Distance = 0f;
                    OnSwipeIndirection?.Invoke(Direction);
                }
                
            }

        }
    }
}