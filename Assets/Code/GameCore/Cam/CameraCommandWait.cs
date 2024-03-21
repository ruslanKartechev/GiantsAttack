using System;
using SleepDev;

namespace GameCore.Cam
{
    public class CameraCommandWait : ICommand<IPlayerCamera>
    {
        private float _time;
        private Action _callback;
        
        public CameraCommandWait(float time, Action callback)
        {
            _time = time;
            _callback = callback;
        }

        public void Execute(IPlayerCamera target, Action onCompleted)
        {
            target.Wait(_time, () =>
            {
                _callback.Invoke();
                onCompleted?.Invoke();
            });
        }
    }
}