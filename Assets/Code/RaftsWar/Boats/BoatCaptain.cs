using System;
using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatCaptain : MonoBehaviour, IBoatCaptain
    {
        private const float WheelPushForce = 25f;
        
        [SerializeField] private float _rotationLerp = .01f;
        [SerializeField] private float _angleLimit = 15f;
        [SerializeField] private Transform _bone;
        [Space(10)]
        [SerializeField] private Transform _rotatable;
        [SerializeField] private Animator _animator;
        [SerializeField] private UnitKilledEffect _killedEffect;
        [SerializeField] private List<Renderer> _unitRenderers;
        [SerializeField] private SinkingConfig _wheelSinkConfig;
        [SerializeField] private SinkingAnimator _wheel;
        private Coroutine _rotating;
        private float _targetAngle;
        
        private void OnEnable()
        {
            _animator.Play("Wheel");
            _targetAngle = 0f;
            _rotating = StartCoroutine(Rotating());
        }

        public void Rotate(Vector3 moveDirection)
        {
            if (moveDirection == Vector3.zero)
                return;
            _rotatable.rotation = Quaternion.Lerp(_rotatable.rotation, 
                Quaternion.LookRotation(moveDirection), GlobalConfig.CaptainRotationLerp);
            var magn = Mathf.Abs(moveDirection.x);
            var sign = Mathf.Sign(moveDirection.x) * Mathf.Sign(moveDirection.y);
            var t = Mathf.InverseLerp(0f, 1f, magn);
            _targetAngle = Mathf.Lerp(0f, _angleLimit, t);
            _targetAngle *= sign;
        }

        public void OnControlRelease()
        {
            _targetAngle = 0f;
        }

        public void SetView(UnitViewSettings viewSettings)
        {
            for (var i = 0; i < viewSettings.unitMaterials.Count; i++)
            {
                var mats = viewSettings.unitMaterials[i];
                mats.Set(_unitRenderers[i]);
            }
        }

        public void DieRagdoll()
        {
            StopCoroutine(_rotating);
            transform.parent = null;
            _killedEffect.PlayKilled();
            BreakWheel();
        }

        private void BreakWheel()
        {
            _wheel.Rb.transform.parent = null;
            _wheel.Rb.isKinematic = false;
            _wheel.Coll.enabled = true;
            _wheel.Rb.AddForce(Vector3.up * WheelPushForce);
            _wheel.Config = _wheelSinkConfig;
            _wheel.IsActive = true;
        }

        private IEnumerator Rotating()
        {
            while (true)
            {
                var eulers = _bone.localEulerAngles;
                var aa = eulers.z;
                if (aa >= 180)
                    aa -= 360;
                aa = Mathf.Lerp(aa, _targetAngle, _rotationLerp);
                eulers.z = aa;
                _bone.localEulerAngles = eulers;
                yield return null;
            }
        }
    }
}