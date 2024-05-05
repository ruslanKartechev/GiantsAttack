using UnityEngine;

namespace SleepDev.Saving
{
    public abstract class IDataSaver : ScriptableObject
    {
        [SerializeField] protected string FileName;
        protected string Path => Application.persistentDataPath + "/" + FileName + ".json";

        public abstract ISavedData GetLoadedData();
        public abstract void Load();
        public abstract void Save();
        public abstract void Clear();
    }
}