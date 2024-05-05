using System.Collections;
using SleepDev;
using UnityEngine;

namespace GameCore
{
    public abstract class Level : MonoExtended
    {
        protected bool _isCompleted;
        protected float _timePassed;
        protected Coroutine _timing;
        
        public abstract void Init();
        public abstract void Win();
        public abstract void Fail();
        public abstract void Pause();
        public abstract void Resume();

        protected void StartTiming()
        {
            StopTiming();
            _timing = StartCoroutine(Timing());
        }

        protected void StopTiming()
        {
            if(_timing != null)
                StopCoroutine(_timing);
        }
        
        private IEnumerator Timing()
        {
            while (true)
            {
                _timePassed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}