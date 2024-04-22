using UnityEngine;

namespace GiantsAttack
{
    public class StageListenerKamikadze : StageListener
    {
        [SerializeField] private float _delay;
        [SerializeField] private SplineEventKamikadze _kamikadze;

        public override void OnActivated()
        {
            Delay(_kamikadze.Activate, _delay);
        }

        public override void OnStopped()
        {
        }

        public override void OnCompleted()
        {
        }
    }
}