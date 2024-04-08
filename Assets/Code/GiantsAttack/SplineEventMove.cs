using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SplineEventMove : SplineEvent
    {
        [SerializeField] private SimpleForwardMover _mover;
        
        public override void Activate()
        {
            _mover.gameObject.SetActive(true);
            _mover.Move(() =>
            {
                _mover.gameObject.SetActive(false);
            });
        }
    }
}