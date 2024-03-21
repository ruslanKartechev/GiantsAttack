using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterShooter : MonoBehaviour, IHelicopterShooter
    {
        [SerializeField] private float _delayBetweenBarrels;
        private Coroutine _shooting;
        private Camera _camera;
        
        public ShooterSettings Settings { get; set; }
        public Transform FromPoint { get; set; }
        public Transform AtPoint { get; set; }
        public IHitCounter HitCounter { get; set; }
        public IHelicopterGun Gun { get; set; }

        public void Init(ShooterSettings settings, IHitCounter hitCounter)
        {
            Settings = settings;
            HitCounter = hitCounter;
            _camera = Camera.main;
        }

        public void StopShooting()
        {
        }

        public void BeginShooting()
        {
        }

        public void RotateToScreenPos(Vector3 aimPos)
        {
            aimPos.z = 50f;
            var endP = _camera.ScreenToWorldPoint(aimPos);
            Debug.DrawLine(Gun.Rotatable.position, endP, Color.red);
            Gun.Rotatable.rotation = Quaternion.LookRotation(endP - Gun.Rotatable.position);
        }
    }
}