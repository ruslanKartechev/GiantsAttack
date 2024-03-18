using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerElectricShooter : MonoBehaviour
    {
        [SerializeField] private Transform _fromPoints;
        [SerializeField] private ElectricProjectile _projectile;
        public Team Team { get; set; }
        public float Damage { get; set; }
        
        public float Speed { get; set; }
        
        public void Fire(ITarget target)
        {
            // CLog.Log($"[{gameObject.name}] Firing at target");
            var proj = Instantiate(_projectile);
            proj.transform.position = _fromPoints.position;
            var damageDealer = new DamageDealer(Damage, _fromPoints.position, target.Damageable, Team);
            proj.Launch(Speed, target,  damageDealer);
        }
    }
}