using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class CameraShaker : MonoBehaviour, ICameraShaker
    {
        [SerializeField] private CameraShakeArgs _defaultArgs;
        [SerializeField] private Transform _movable;
        private Coroutine _working;
        public void Play(CameraShakeArgs args)
        {
            Stop();
            _working = StartCoroutine(Working(args));
        }

        public void PlayDefault()
        {
            Play(_defaultArgs);   
        }

        public void Stop()
        {
            if(_working != null)
                StopCoroutine(_working);
        }

        private IEnumerator Working(CameraShakeArgs args)
        {
            var elapsed = 0f;
            var timeStep = 1f / args.freqDefault;
            while (elapsed <= args.durationDefault)
            {
                var pos = UnityEngine.Random.onUnitSphere * args.forceDefault;
                _movable.localPosition = pos;
                yield return new WaitForSeconds(timeStep);
                elapsed += timeStep;
            }
            _movable.localPosition = Vector3.zero;
        }
    }
}