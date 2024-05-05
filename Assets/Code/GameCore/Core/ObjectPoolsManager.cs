using GiantsAttack;
using SleepDev;
using UnityEngine;

namespace GameCore.Core
{
    public class ObjectPoolsManager : MonoBehaviour, IObjectPoolsManager
    {
        private static bool _inited;
        [SerializeField] private int _startBulletsPoolSize = 100;
        [SerializeField] private BulletsPool _bulletsPool; 
        [SerializeField] private BulletCasingPool _casingsPool; 

        public void BuildPools()
        {
            CLog.Log($"[Pools manager] Build pools call");
            if (_inited)
            {
                CLog.Log($"[Pools manager] already intied");
                Destroy(gameObject);
                return;
            }
            _inited = true;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            _bulletsPool.GOFactory = _casingsPool.GOFactory = GCon.GOFactory;
            _bulletsPool.BuildPool(_startBulletsPoolSize);
            // _casingsPool.BuildPool(_startBulletsPoolSize);
            // Setup container
            GCon.PoolsManager = this;
        }

        public void RecollectAll()
        {
            CLog.Log($"[PoolsManager] RecollectAll");
        }

        public IObjectPool<IBullet> BulletsPool => _bulletsPool;
        public IObjectPool<BulletCasing> BulletCasingsPool => _casingsPool;
    }
}