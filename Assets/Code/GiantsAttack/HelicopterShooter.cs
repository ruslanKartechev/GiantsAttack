﻿using System.Collections;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterShooter : MonoBehaviour, IHelicopterShooter
    {
        [SerializeField] private byte _countPerBarrel = 100;
        [SerializeField] private Transform _shootDirection;
        [SerializeField] private HelicopterAnimatedDisplay _display;
        [SerializeField] private SoundSo _fireSound;
        private byte[] _barrelCounts;
        private Coroutine _shooting;
        private Camera _camera;
        private Coroutine _working;
        private bool _isShooting;
        private bool _isReloading;

        public ShooterSettings Settings { get; set; }
        
        public IHitCounter HitCounter { get; set; }
        public IHelicopterGun Gun { get; set; }

        public void Init(ShooterSettings settings, IHitCounter hitCounter)
        {
            Settings = settings;
            HitCounter = hitCounter;
            _camera = Camera.main;
            _barrelCounts = new byte[Gun.Barrels.Count];
            UpdatePerBarrelCounts();
        }

        public void StopShooting()
        {
            _isShooting = false;
            StopShootingLoop();
        }

        public void BeginShooting()
        {
            StopShootingLoop();
            _isShooting = true;
            _working = StartCoroutine(Shooting());
            foreach (var barrel in Gun.Barrels)
                barrel.Rotate(true);
        }
        
        public void RotateToScreenPos(Vector3 aimPos)
        {
            const float camDepth = 150;
            aimPos.z = (_camera.transform.position - _shootDirection.position).magnitude + camDepth; 
            _shootDirection.rotation = Quaternion.LookRotation(
                _camera.ScreenToWorldPoint(aimPos) - _shootDirection.position);
        }

        private void StopShootingLoop()
        {
            if(_working != null)
                StopCoroutine(_working);
            foreach (var barrel in Gun.Barrels)
                barrel.Rotate(false);
        }
        
        private void UpdatePerBarrelCounts()
        {
            for (byte index = 0; index < _barrelCounts.Length; index++)
            {
                _barrelCounts[index] = _countPerBarrel;
                _display.ShowNormalCount(index);
                _display.SetBulletsCount(index, _countPerBarrel);
            }
        }

        private void MinusCount(byte index)
        {
            if(_barrelCounts[index] != 0)
                _barrelCounts[index]--;
            if(_barrelCounts[index] < 10)
                _display.ShowLowCount(index);
            _display.SetBulletsCount(index, _barrelCounts[index]);
        }

        private bool CheckReload()
        {
            foreach (var count in _barrelCounts)
            {
                if (count > 0)
                    return false;
            }
            return true;
        }

        private void Reload()
        {
            StopShootingLoop();
            Gun.PlayReload(OnReloaded);
        }

        private void OnReloaded()
        {
            UpdatePerBarrelCounts();
            if(_isShooting)
                BeginShooting();
        }

        private IEnumerator Shooting()
        {
            var didShoot = false;
            var it = 0;
            var it_max = 10;
            while (it < it_max)
            {
                didShoot = false;
                it++;
                for (byte i = 0; i < Gun.Barrels.Count; i++)
                {
                    if(_barrelCounts[i] == 0)
                        continue;
                    didShoot = true;
                    it = 0;
                    var barrel = Gun.Barrels[i];
                    var bullet = GCon.PoolsManager.BulletsPool.GetObject();
                    bullet.SetRotation(barrel.FromPoint.rotation);
                    var damage = Settings.damage.Random();
#if UNITY_EDITOR
                    damage *= GlobalConfig.DamageMultiplier;
#endif
                    var args = new DamageArgs() {damage = damage};
                    bullet.Scale(1f);
                    bullet.Launch(barrel.FromPoint.position, _shootDirection.forward, speed: Settings.speed, args, HitCounter);
                    barrel.Recoil();
                    _fireSound.Play();
                    MinusCount(i);
                    if(CheckReload())
                        Reload();
                    yield return new WaitForSeconds(Settings.fireDelay);
                }
                if (didShoot == false)
                    yield return null;
            }
        }
        
    }
}