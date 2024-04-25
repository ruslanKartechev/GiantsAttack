using System;
using System.Collections.Generic;
using GameCore.UI;
using SleepDev;
using SleepDev.Sound;
using UnityEngine;

namespace GiantsAttack
{
    public class Helicopter : MonoBehaviour, IHelicopter
    {
        [SerializeField] private GameObject _gun;
        [SerializeField] private PlayerAltitudeMeter _altitudeMeter;
        [SerializeField] private PlayerCompas _compas;
        [SerializeField] private BodySectionsUI _bodySectionsUI;
        [SerializeField] private List<ParticleSystem> _bladeParticles;
        [SerializeField] private Transform _internalTransform;
        [SerializeField] private SoundSo _helicopterSound;
        [SerializeField] private Light _light;   
        private PlayingSound _playingSound;
        private bool _isDead;

        public IHelicopterMover Mover { get; private set;}
        public IHelicopterShooter Shooter { get; private set; }
        public IHelicopterAimer Aimer { get; private set;}
        public IDamageable Damageable { get; private set; }
        public IHelicopterCameraPoints CameraPoints { get; private set; }
        public IDestroyer Destroyer { get; private set; }
        public IBodySectionsUI BodySectionsUI => _bodySectionsUI;
        public Transform Point => _internalTransform;

        public void Init(HelicopterInitArgs args)
        {
            Mover = GetComponent<IHelicopterMover>();
            Shooter = GetComponent<IHelicopterShooter>();
            Aimer = GetComponent<IHelicopterAimer>();
            CameraPoints = GetComponent<IHelicopterCameraPoints>();
            Destroyer = GetComponent<IDestroyer>();

            Damageable = GetComponent<HelicopterHealth>();
            Aimer.Init(args.aimerSettings, Shooter, args.controlsUI, args.aimUI);
            var gun = _gun.GetComponent<IHelicopterGun>();
            Shooter.Gun = gun;
            Shooter.Init(args.shooterSettings, args.hitCounter);
            CameraPoints.SetCamera(args.camera);
            _altitudeMeter.Begin();
            _compas.BeginTracking(args.enemyTransform);
            _playingSound = _helicopterSound.Play();
            // if (EnvironmentState.IsNight)
            //     _light.enabled = true;
            // else
            //     _light.enabled = false;
        }

        public void StopAll()
        {
            Shooter.StopShooting();
            Aimer.StopAim();
        }
        
        public void Kill()
        {
            if (_isDead)
            {
                CLog.Log($"Helicopter already dead");
                return;
            }

            foreach (var particle in _bladeParticles)
                particle.gameObject.SetActive(false);
            _isDead = true;
            Mover.StopAll();
            Aimer.StopAim();
            Shooter.StopShooting();
            Shooter.Gun.StopAnimations();
            Destroyer.DestroyMe();
            StopAll();
        }

    }
}