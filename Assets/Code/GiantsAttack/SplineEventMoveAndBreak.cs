using System.Collections;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class SplineEventMoveAndBreak : SplineEvent
    {
        [SerializeField] private bool _doMove = true;
        [SerializeField] private AnimatedTarget _animatedVehicle;
        [SerializeField] private ExplosiveVehicle _explosive;
        [SerializeField] private float _hideDelay;

        public override void Activate()
        {
            gameObject.SetActive(true);
            if (_doMove)
            {
                _animatedVehicle.Move(_explosive.Explode);                
            }
            else
            {
                _explosive.Explode();    
            }
            StartCoroutine(DelayedHide());
        }

        private IEnumerator DelayedHide()
        {
            yield return new WaitForSeconds(_hideDelay);
            _animatedVehicle.gameObject.SetActive(false);
        }
    }
}