using System;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterMoveToData
    {
        public Transform endPoint;
        public float time;
        public AnimationCurve curve;
        public Action callback;
     
        public HelicopterMoveToData(Transform endPoint, float time, AnimationCurve curve, Action callback)
        {
            this.endPoint = endPoint;
            this.time = time;
            this.curve = curve;
            this.callback = callback;
        }
        
        public bool HasStarted { get; set; }
        public float LerpT { get; set; }
        public Vector3 StartPos { get; set; }
        public Quaternion StartRot { get; set; }

        public void RefreshStartPosAndRot(Transform point)
        {
            StartPos = point.position;
            StartRot = point.rotation;
            LerpT = 0f;
        }
    }
}