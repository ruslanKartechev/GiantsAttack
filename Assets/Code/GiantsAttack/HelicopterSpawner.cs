using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public class HelicopterSpawner
    {
        public IHelicopter SpawnAt(Transform point, Transform parent)
        {
            var prefab = Resources.Load<GameObject>("Prefabs/PlayerHelicopter");
            var go = UnityEngine.Object.Instantiate(prefab);
            go.transform.SetParent(parent);
            go.transform.CopyPosRot(point);
            var inst = go.GetComponent<IHelicopter>();
            
            return inst;
        }
    }
}