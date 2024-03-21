using UnityEngine;

namespace SleepDev.Pooling
{
    public interface IPooledObject<T> where T : class
    {
        IObjectPool<T> Pool { get; set; }
        void Parent(Transform parent);
        T Obj { get; }
        void Destroy();
        void Hide();
    }
}