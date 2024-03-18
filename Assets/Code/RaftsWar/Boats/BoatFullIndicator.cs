using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BoatFullIndicator : MonoExtended
    {
        [SerializeField] private Animator _animator;
        private bool _isPlaying;
        public void Play()
        {
            if (_isPlaying)
            {
                StopDelayedAction();
                Delay(Stop, GlobalConfig.PlayerFullIndicatorDuration);
                return;
            }
            _isPlaying = true;
            _animator.enabled = true;
            _animator.gameObject.SetActive(true);
            _animator.Play("Play");
            Delay(Stop, GlobalConfig.PlayerFullIndicatorDuration);
        }

        public void Stop()
        {
            _isPlaying =false;   
            StopDelayedAction();
            _animator.gameObject.SetActive(false);
        }
        
    }
}