using System;
using System.Collections;
using UnityEngine;

namespace SleepDev
{
    public class MonoExtended : MonoBehaviour
    {
        protected Coroutine _delayedAction;

        protected void Delay(Action callback, float time)
        {
            StopDelayedAction();
            _delayedAction = StartCoroutine(DelayedAction(time, callback));
        }

        protected void StopDelayedAction()
        {
            if(_delayedAction != null)
                StopCoroutine(_delayedAction);
        }
        
        protected IEnumerator DelayedAction(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }
    }
}