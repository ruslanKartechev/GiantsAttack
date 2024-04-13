using System.Collections.Generic;
using GameCore.Cam;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterDestroyer : MonoBehaviour, IDestroyer
    {
        [SerializeField] private float _cameraSetTime = .5f;
        [SerializeField] private List<MonoBehaviour> _rotators;
        [SerializeField] private ByPartsDestroyer _byParts;
        [SerializeField] private ParticleSystem _particles;

        
        public void DestroyMe()
        {
            foreach (var rot in _rotators)
                rot.enabled = false;
            _particles.Play();
            _byParts.BreakAll(() => {});
            var helicopter = gameObject.GetComponent<IHelicopter>();
            helicopter.Aimer.StopAim();
            helicopter.Shooter.StopShooting();
            helicopter.Mover.StopAll();
            CameraContainer.PlayerCamera.MoveToPoint(helicopter.CameraPoints.OutsidePoint, 
                _cameraSetTime,() => {});
        }
    }
}