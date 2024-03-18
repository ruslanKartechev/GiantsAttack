namespace SleepDev.Data
{
    [System.Serializable]
    public class DataByType<TData, TType>
    {
        public TData data;
        public TType type;
    }
}