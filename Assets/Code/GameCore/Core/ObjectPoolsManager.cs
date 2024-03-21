using SleepDev;
using UnityEngine;

namespace GameCore.Core
{
    public class ObjectPoolsManager : MonoBehaviour, IObjectPoolsManager
    {
        private static bool _inited;

        public void BuildPools()
        {
            if (_inited)
            {
                Destroy(gameObject);
                return;
            }

            _inited = true;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            // Setup container
            GCon.PoolsManager = this;
        }

        public void RecollectAll()
        {
            CLog.Log($"[PoolsManager] RecollectAll");
        }
    }
}