using System;
using System.Collections;
using GameCore.Core;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class TankShooter : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] private Transform _rotatable;
        [SerializeField] private float _rotationSpeed;
        [Header("Projectiles")] 
        [SerializeField] private float _projectileScale = 2f;
        [SerializeField] private ParticleSystem _flash;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private float _defaultShootingDelay;
        [SerializeField] private float _defaultProjectileSpeed;
        [Header("Recoil")]
        [SerializeField] private float _recoilAngle;
        [SerializeField] private Vector3 _recoilRollTime;
        private Coroutine _working;

        public void ShootOnceInDir(Transform lookAt)
        {
            Stop();
            _working = StartCoroutine(RotatingToLook(lookAt, ShootOnce));
        }        
        
        public void ShootOnce()
        {
            Stop();
            _working = StartCoroutine(ShootingOnce(() => {}));
        }

        public void ShootLoop()
        {
            Stop();
            _working = StartCoroutine(ShootingLoop(_defaultShootingDelay));
        }

        private void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private void Shoot()
        {
            var bullet = GCon.PoolsManager.BulletsPool.GetObject();
            bullet.Scale(_projectileScale);
            bullet.LaunchBlank(_muzzle.position, _muzzle.forward, _defaultProjectileSpeed);
            _flash.Play();
        }

        private IEnumerator RotatingToLook(Transform point, Action onEnd)
        {
            var r1 = _rotatable.rotation;
            var r2 = Quaternion.LookRotation((point.position - _rotatable.position).XZPlane());
            var time = (Quaternion.Angle(r2, r1)) / _rotationSpeed;
            var elapsed = 0f;
            while (elapsed < time)
            {
                _rotatable.rotation = Quaternion.Lerp(r1, r2, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _rotatable.rotation = r2;
            onEnd.Invoke();
        }


        private IEnumerator ShootingOnce(Action onEnd)
        {
            Shoot();
            yield return Recoiling();
            onEnd.Invoke();
        }
        
        private IEnumerator ShootingLoop(float delay)
        {
            while (true)
            {
                yield return ShootingOnce(() => {});
                yield return new WaitForSeconds(delay);
            }
        }

        private IEnumerator Recoiling()
        {
            var r1 = _rotatable.rotation;
            var r2 = r1 * Quaternion.Euler(_recoilAngle, 0f, 0f);
            var r3 = r1 * Quaternion.Euler(-_recoilAngle * .5f, 0f, 0f);
            var time = _recoilRollTime.x;
            var elapsed = Time.deltaTime;
            while (elapsed < time)
            {
                _rotatable.rotation = Quaternion.Lerp(r1, r2, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _rotatable.rotation = r2;
            elapsed = Time.deltaTime;
            time = _recoilRollTime.y;
            while (elapsed < time)
            {
                _rotatable.rotation = Quaternion.Lerp(r2, r3, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = Time.deltaTime;
            time = _recoilRollTime.z;
            while (elapsed < time)
            {
                _rotatable.rotation = Quaternion.Lerp(r3, r1, elapsed / time);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _rotatable.rotation = r1;
        }

    }
}