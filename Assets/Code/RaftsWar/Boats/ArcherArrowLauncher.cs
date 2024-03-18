using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class ArcherArrowLauncher : MonoBehaviour
    {
        [SerializeField] private Transform _shootFrom;
        public Team Team { get; set; }
        public ITarget CurrentTarget { get; set; }
        public UnitViewSettings ViewSettings { get; set; }
        
        public float Damage { get; set; }
        
        // Animation event
        public void OnShootEvent()
        {
            var arrow = GCon.ArrowsPool.GetObject();
            arrow.Go.transform.SetPositionAndRotation(_shootFrom.position, _shootFrom.rotation);
            arrow.Go.transform.parent = transform;
            arrow.SetView(ViewSettings);
            arrow.Launch(GlobalConfig.ArrowSpeed, CurrentTarget,
                new DamageDealer(Damage, transform.position, CurrentTarget.Damageable, Team));
        }
    }
}