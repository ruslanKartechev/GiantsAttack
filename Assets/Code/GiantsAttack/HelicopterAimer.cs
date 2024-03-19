using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterAimer : MonoBehaviour, IHelicopterAimer
    {
        [SerializeField] private Transform _fromPoint;
        [SerializeField] private Transform _atPoint;
        private Coroutine _aiming;

        public AimerSettings Settings { get; set; }
        public void Init(AimerSettings settings, IHelicopterShooter shooter)
        {
            throw new System.NotImplementedException();
        }

        public void BeginAim()
        {
            throw new System.NotImplementedException();
        }

        public void StopAim()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}