using System.Collections.Generic;
using SleepDev;
using SleepDev.Pooling;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class CatapultProjectilePool : MonoBehaviour, IObjectPool<ICatapultProjectile>
    {
        private const int InitCapacity = 50;
        private const int InitPoolSize = 20;
        
        [SerializeField] private Transform _parent;
        
        private List<IPooledObject<ICatapultProjectile>> _pool = new List<IPooledObject<ICatapultProjectile>>();
        private List<IPooledObject<ICatapultProjectile>> _allSpawned = new List<IPooledObject<ICatapultProjectile>>();

        public void Init()
        {
            _pool = new List<IPooledObject<ICatapultProjectile>>(InitCapacity);
            _allSpawned = new List<IPooledObject<ICatapultProjectile>>(InitCapacity);
            BuildPool(InitPoolSize);
        }
        
        public void RecollectAll()
        {
            CLog.LogWhite($"[CatapultProjPool] Recollecting All back");
            _pool.Clear();
            foreach (var obj in _allSpawned)
            {
                obj.Target.Reset();
                obj.Target.Go.transform.parent = _parent;
                _pool.Add(obj);
            }
        }

        public void BuildPool(int size)
        {
            for (var i = 0; i < size; i++)
            {
                var obj = GCon.GOFactory.Spawn<IPooledObject<ICatapultProjectile>>(GlobalConfig.CatapultProjectileID);
                obj.Pool = this;
                _pool.Add(obj);
                _allSpawned.Add(obj);
                obj.Target.Go.transform.parent = _parent;
                obj.Target.Go.SetActive(false);
            }
        }

        public ICatapultProjectile GetObject()
        {
            if(_pool.Count == 0)
                BuildPool(InitPoolSize / 2);
            var p = _pool[^1];
            _pool.Remove(p);
            return p.Target;
        }

        public void ReturnObject(IPooledObject<ICatapultProjectile> obj)
        {
            _pool.Add(obj);
        }

        public void ClearPool()
        {
        }

        public ICatapultProjectile[] GetObjects(int count)
        {
            return null;
        }

        public int CurrentSize => _pool.Count;
    }
}