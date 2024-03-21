using System.Collections.Generic;
using SleepDev;

namespace GameCore.Cam
{
    public class CameraCommandsHolder : ICommandsHolder<IPlayerCamera>
    {
        private Queue<ICommand<IPlayerCamera>> _queue = new Queue<ICommand<IPlayerCamera>>(10);
        public int Count => _queue.Count;
        
        public void Clear()
        {
            _queue.Clear();
        }

        public void Add(ICommand<IPlayerCamera> command)
        {
            // CLog.LogYellow($"[CameraCommandHolder] Command added");
            _queue.Enqueue(command);
        }

        public ICommand<IPlayerCamera> GetLast()
        {
            return _queue.Dequeue();
        }

        public ICommand<IPlayerCamera> GetFirst()
        {
            return _queue.Dequeue();
        }
    }
}