using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterShooter : MonoBehaviour, IHelicopterShooter
    {
        private Coroutine _shooting;
        
        public ShooterSettings Settings { get; set; }
        public Transform FromPoint { get; set; }
        public Transform AtPoint { get; set; }
        public IHitCounter HitCounter { get; set; }

        public void Init(ShooterSettings settings, IHitCounter hitCounter)
        {
            Settings = settings;
            HitCounter = hitCounter;
        }

        public void StopShooting()
        {
        }

        public void BeginShooting()
        {
        }
    }
}