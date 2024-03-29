﻿using System.Collections.Generic;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class Helicopter : MonoBehaviour, IHelicopter
    {
        [SerializeField] private GameObject _gun;
        [SerializeField] private List<Transform> _rocketPoint;
        [SerializeField] private Transform _rocketCamPoint;
        [SerializeField] private PlayerAltitudeMeter _altitudeMeter;
        [SerializeField] private PlayerCompas _compas;
        [SerializeField] private BodySectionsUI _bodySectionsUI;
        private bool _isDead;

        public IHelicopterMover Mover { get; private set;}
        public IHelicopterShooter Shooter { get; private set; }
        public IHelicopterAimer Aimer { get; private set;}
        public IDamageable Damageable { get; private set; }
        public IHelicopterCameraPoints CameraPoints { get; private set; }
        public IDestroyer Destroyer { get; private set; }
        public IBodySectionsUI BodySectionsUI => _bodySectionsUI;
        public List<Transform> RocketPoints => _rocketPoint;
        public Transform RocketCamPoint => _rocketCamPoint;

        public void Init(HelicopterInitArgs args)
        {
            Mover = GetComponent<IHelicopterMover>();
            Shooter = GetComponent<IHelicopterShooter>();
            Aimer = GetComponent<IHelicopterAimer>();
            CameraPoints = GetComponent<IHelicopterCameraPoints>();
            Destroyer = GetComponent<IDestroyer>();

            Mover.Settings = args.moverSettings;
            Damageable = GetComponent<HelicopterHealth>();
            Aimer.Init(args.aimerSettings, Shooter, args.controlsUI, args.aimUI);
            Shooter.Init(args.shooterSettings, args.hitCounter);
            var gun = _gun.GetComponent<IHelicopterGun>();
            Shooter.Gun = gun;
            CameraPoints.SetCamera(args.camera);
            _altitudeMeter.Begin();
            _compas.BeginTracking(args.enemyTransform);
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
            _isDead = true;
            Mover.StopAll();
            Aimer.StopAim();
            Shooter.StopShooting();
            Destroyer.DestroyMe();
        }

    }
}