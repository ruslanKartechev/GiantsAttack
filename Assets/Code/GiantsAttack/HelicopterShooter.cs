using System.Collections;
using GameCore.Core;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterShooter : MonoBehaviour, IHelicopterShooter
    {
        [SerializeField] private float _delayBetweenBarrels;
        [SerializeField] private Transform _shootDirection;
        private Coroutine _shooting;
        private Camera _camera;
        private Coroutine _working;
        
        public ShooterSettings Settings { get; set; }
        public Transform FromPoint { get; set; }
        public Transform AtPoint { get; set; }
        
        public IHitCounter HitCounter { get; set; }
        public IDamageHitsUI DamageHitsUI { get; set; }
        public IHelicopterGun Gun { get; set; }
        

        public void Init(ShooterSettings settings, IHitCounter hitCounter)
        {
            Settings = settings;
            HitCounter = hitCounter;
            _camera = Camera.main;
        }

        public void StopShooting()
        {
            if(_working != null)
                StopCoroutine(_working);
            foreach (var barrel in Gun.Barrels)
                barrel.Rotate(false);
        }

        public void BeginShooting()
        {
            StopShooting();
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

        private IEnumerator Shooting()
        {
            while (true)
            {
                foreach (var barrel in Gun.Barrels)
                {
                    var bullet = GCon.PoolsManager.BulletsPool.GetObject();
                    bullet.SetRotation(barrel.FromPoint.rotation);
                    float damage = 0f;
                    var chance = UnityEngine.Random.Range(0f, 1f);
                    var args = new DamageArgs();
                    if (chance < Settings.critChance)
                    {
                        damage = Settings.critDamage;
                        args.isCrit = true;
                    }
                    else
                        damage = Settings.damage.Random();
#if UNITY_EDITOR
                    damage *= GlobalConfig.DamageMultiplier;
#endif
                    args.damage = damage;
                    bullet.Scale(1f);
                    bullet.Launch(barrel.FromPoint.position, _shootDirection.forward, 
                        speed:Settings.speed, args, HitCounter, DamageHitsUI);
                    var casing = GCon.PoolsManager.BulletCasingsPool.GetObject();
                    casing.Drop(barrel.DropPoint);
                    barrel.Recoil();
                    yield return new WaitForSeconds(_delayBetweenBarrels);
                }
                yield return new WaitForSeconds( Settings.fireDelay);
            }
        }
        
    }
}