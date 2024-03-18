using System.Collections.Generic;
using SleepDev;
using SleepDev.Pooling;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class ArrowsPool : MonoBehaviour, IObjectPool<IArrow>
    {
        private const int InitCapacity = 100;
        private const int InitPoolSize = 100;
        
        [SerializeField] private Transform _parent;
        
        private List<IPooledObject<IArrow>> _pool = new List<IPooledObject<IArrow>>();
        private List<IPooledObject<IArrow>> _allSpawned = new List<IPooledObject<IArrow>>();

        public void Init()
        {
            _pool = new List<IPooledObject<IArrow>>(InitCapacity);
            _allSpawned = new List<IPooledObject<IArrow>>(InitCapacity);
            BuildPool(InitPoolSize);
        }
        
        public void RecollectAll()
        {
            CLog.LogWhite($"[CatapultProjPool] Recollecting All back");
            _pool.Clear();
            foreach (var obj in _allSpawned)
            {
                obj.Target.Reset();
                _pool.Add(obj);
                obj.Target.Go.transform.parent = _parent;
            }
        }


        public void BuildPool(int size)
        {
            for (var i = 0; i < size; i++)
            {
                var obj = GCon.GOFactory.Spawn<IPooledObject<IArrow>>(GlobalConfig.ArrowID);
                obj.Pool = this;
                _pool.Add(obj);
                _allSpawned.Add(obj);
                obj.Target.Go.transform.parent = _parent;
                obj.Target.Go.SetActive(false);
#if UNITY_EDITOR
                obj.Target.Go.name = $"arrow_{i}";
#endif
            }
        }

        public IArrow GetObject()
        {
            if(_pool.Count == 0)
                BuildPool(InitPoolSize / 2);
            var p = _pool[^1];
            _pool.Remove(p);
            return p.Target;
        }

        public void ReturnObject(IPooledObject<IArrow> obj)
        {
            // CLog.LogRed($"*************** ARROW RETURNED TO POOl");
            _pool.Add(obj);
        }

        public void ClearPool()
        { }

        public IArrow[] GetObjects(int count)
        {
            return null;
        }

        public int CurrentSize => _pool.Count;
    }
}