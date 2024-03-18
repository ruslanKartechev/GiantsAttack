using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class ObjectPoolsManager : MonoBehaviour, IObjectPoolsManager
    {
        [SerializeField] private BoatPartsPool _raftsPoolDefault;
        [SerializeField] private BoatPartsPool _raftsPoolUnits;
        [SerializeField] private CatapultProjectilePool _catapultProjectiles;
        [SerializeField] private ArrowsPool _arrowsPool;
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
            GCon.BoatPartsPool = _raftsPoolDefault;
            GCon.BoatPartsPoolsWithUnits = _raftsPoolUnits;
            GCon.CatapultProjectilesPool = _catapultProjectiles;
            GCon.ArrowsPool = _arrowsPool;
            // setup ids
            _raftsPoolDefault.ItemID = GlobalConfig.BoatPartID;
            _raftsPoolUnits.ItemID = GlobalConfig.BoatPartWithUnitID;
            // initiate pools
            _catapultProjectiles.Init();
            _raftsPoolDefault.Init();
            _raftsPoolUnits.Init();
            _arrowsPool.Init();
        }

        public void RecollectAll()
        {
            _raftsPoolDefault.RecollectAll();
            _catapultProjectiles.RecollectAll();
            _arrowsPool.RecollectAll();
        }
    }
}