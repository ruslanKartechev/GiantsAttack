using System.Collections.Generic;
using SleepDev;
using SleepDev.Pooling;
using UnityEngine;

namespace GiantsAttack
{
    public class BulletsPool : MonoBehaviour, IObjectPool<IBullet>
    {
        [SerializeField] private int _extensionSize = 50;
        [SerializeField] private string _id;
        private List<IPooledObject<IBullet>> _instances = new List<IPooledObject<IBullet>>();
        private GameObjectFactory _goFactory;

        public GameObjectFactory GOFactory
        {
            get => _goFactory;
            set => _goFactory = value;
        }

        public string ID
        {
            get => _id;
            set => _id = value;
        }
        public int CurrentSize => _instances.Count;

        public void BuildPool(int size)
        {
            var objs = GOFactory.Spawn<IPooledObject<IBullet>>(ID, size);
            foreach (var ob in objs)
            {
                ob.Pool = this;
                ob.Parent(transform);
                ob.Hide();
                _instances.Add(ob);
            }
        }

        public IBullet GetObject()
        {
            if (_instances.Count == 0)
            {
                CLog.Log($"Extending bullets pool");
                BuildPool(_extensionSize);
            }
            var item = _instances[^1];
            _instances.RemoveAt(_instances.Count-1);
            return item.Obj;
        }

        public void ReturnObject(IPooledObject<IBullet> obj)
        {
            _instances.Add(obj);
        }

        public void ClearPool()
        {
            foreach (var instance in _instances)
                instance.Destroy();
        }

        public IBullet[] GetObjects(int count)
        {
            var arr = new IBullet[count];
            for (var i = 0; i < count; i++)
                arr[i] = GetObject();
            return arr;
        }

    }
}