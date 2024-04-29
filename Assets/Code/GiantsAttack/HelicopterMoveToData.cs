﻿using System;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterMoveToData
    {
        public Transform endPoint;
        public Transform lookAt;
        public float time;
        public AnimationCurve curve;
        public Action callback;
     
        public HelicopterMoveToData(Transform endPoint, float time, AnimationCurve curve, Transform lookAt, Action callback)
        {
            this.lookAt = lookAt;
            this.endPoint = endPoint;
            this.time = time;
            this.curve = curve;
            this.callback = callback;
        }
        
        public bool HasStarted { get; set; }
        public bool HasFinished { get; set; }
        
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