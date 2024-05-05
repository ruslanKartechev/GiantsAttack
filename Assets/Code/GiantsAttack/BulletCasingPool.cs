using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class BulletCasingPool : MonoBehaviour, IObjectPool<BulletCasing>
    {
        [SerializeField] private int _extensionSize = 50;
        [SerializeField] private string _id;
        private List<IPooledObject<BulletCasing>> _instances = new List<IPooledObject<BulletCasing>>();
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
            var objs = GOFactory.Spawn<IPooledObject<BulletCasing>>(ID, size);
            foreach (var ob in objs)
            {
                ob.Pool = this;
                ob.Parent(transform);
                ob.Hide();
                _instances.Add(ob);
            }
        }

        public BulletCasing GetObject()
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

        public void ReturnObject(IPooledObject<BulletCasing> obj)
        {
            _instances.Add(obj);
        }

        public void ClearPool()
        {
            foreach (var instance in _instances)
                instance.Destroy();
        }

        public BulletCasing[] GetObjects(int count)
        {
            var arr = new BulletCasing[count];
            for (var i = 0; i < count; i++)
                arr[i] = GetObject();
            return arr;
        }
    }
}