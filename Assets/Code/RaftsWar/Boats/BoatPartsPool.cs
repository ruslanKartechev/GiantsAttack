using System.Collections.Generic;
using SleepDev;
using SleepDev.Pooling;
using UnityEngine;

namespace RaftsWar.Boats
{
    
    public class BoatPartsPool : MonoBehaviour, IObjectPool<BoatPart>
    {
        private const int ExtensionSize = 15;
        private const int InitialSize = 30;
        
        [SerializeField] private string _id;
        [SerializeField] private Transform _parent;
        private Queue<IPooledObject<BoatPart>> _pool = new Queue<IPooledObject<BoatPart>>(InitialSize);
        private Queue<IPooledObject<BoatPart>> _allSpawned = new Queue<IPooledObject<BoatPart>>(InitialSize);
        
        private GameObjectFactory _factory;

        public string ItemID
        {
            get => _id;
            set => _id = value;
        }
        
        public void Init()
        {
            _factory = GCon.GOFactory;
            BuildPool(InitialSize);
        }

        public void RecollectAll()
        {
            CLog.LogWhite($"[BoatpartsPool] Recollecting All back");
            _pool.Clear();
            foreach (var obj in _allSpawned)
            {
                obj.Target.HideForPool();
                obj.Target.transform.parent = _parent;
                _pool.Enqueue(obj);
            }
        }
        
        public void BuildPool(int size)
        {
            var spawned = _factory.Spawn<IPooledObject<BoatPart>>(ItemID, size);
            var ind = 0;
            foreach (var obj in spawned)
            {
                obj.Pool = this;
                obj.Target.gameObject.SetActive(false);
                obj.Target.transform.SetParent(_parent);
#if UNITY_EDITOR
                obj.Target.gameObject.name = $"bp_{ind};";
#endif
                ind++;
                _pool.Enqueue(obj);
                _allSpawned.Enqueue(obj);
                // _pool.Add(obj);
                // _allSpawned.Add(obj);
            }
        }

        public BoatPart GetObject()
        {
            if (_pool.Count == 0)
            {
                BuildPool(ExtensionSize);
            }
            var item = _pool.Dequeue();
            if (item == null)
            {
                CLog.LogRed($"[BoatPartsPool] null dequeued");
                item = _pool.Dequeue();
            }
            return item.Target;
        }

        public void ReturnObject(IPooledObject<BoatPart> obj)
        {
            // _pool.Add(obj);
            _pool.Enqueue(obj);
        }

        public void ClearPool()
        {
            foreach (var instance in _pool)
            {
                instance.Destroy();
            }
            _pool.Clear();
        }

        public BoatPart[] GetObjects(int count)
        {
            var arr = new BoatPart[count];
            for (var i = 0; i < count; i++)
            {
                arr[i] = GetObject();
            }
            return arr;
        }

        public int CurrentSize => _pool.Count;
    }
}