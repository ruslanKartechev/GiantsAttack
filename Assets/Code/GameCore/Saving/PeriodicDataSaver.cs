using System.Collections;
using UnityEngine;

namespace SleepDev.Saving
{
    public class PeriodicDataSaver : MonoBehaviour, IPeriodicDataSaver
    {
        [SerializeField] private IDataSaver _dataSaver;
        private float _interval = 5;
        
        public void SetInterval(float interval) => _interval = interval;
        
        public void Begin()
        {
            StartCoroutine(Saving());
        }

        private IEnumerator Saving()
        {
            yield return null;
            while (true)
            {
                yield return new WaitForSecondsRealtime(_interval);
                _dataSaver.Save();
            }
        }
    }
}