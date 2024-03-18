namespace SleepDev.Pooling
{
    public interface IPooledObject<T> where T : class
    {
        IObjectPool<T> Pool { get; set; }
        void Destroy();
        T Target { get; }
    }
}