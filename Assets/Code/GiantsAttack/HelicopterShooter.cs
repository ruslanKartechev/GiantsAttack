using System.Collections;
using GameCore.Core;
using GameCore.UI;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterShooter : MonoBehaviour, IHelicopterShooter
    {
        [SerializeField] private float _delayBetweenBarrels;
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
        }

        public void BeginShooting()
        {
            StopShooting();
            _working = StartCoroutine(Shooting());
        }

        public void RotateToScreenPos(Vector3 aimPos)
        {
            aimPos.z = (_camera.transform.position - Gun.Rotatable.position).magnitude + 200; 
            var endP = _camera.ScreenToWorldPoint(aimPos);
            // Debug.DrawLine(endP, Gun.Rotatable.position, Color.black, 5f);
            Gun.Rotatable.rotation = Quaternion.LookRotation(endP - Gun.Rotatable.position);
        }

        private IEnumerator Shooting()
        {
            while (true)
            {
                foreach (var barrel in Gun.Barrels)
                {
                    var bullet = GCon.BulletsPool.GetObject();
                    bullet.SetRotation(barrel.FromPoint.rotation);
                    bullet.Launch(barrel.FromPoint.position, barrel.FromPoint.forward, 
                        speed:Settings.speed, damage:Settings.damage, 
                        HitCounter, DamageHitsUI);
                    barrel.Recoil();
                    yield return new WaitForSeconds(_delayBetweenBarrels);
                }
                yield return new WaitForSeconds( Settings.fireDelay);
            }
        }
        
    }
}