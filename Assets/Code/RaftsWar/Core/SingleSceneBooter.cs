using RaftsWar.Boats;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Core
{
    [DefaultExecutionOrder(-100)]
    public class SingleSceneBooter : MonoBehaviour
    {
        private static SingleSceneBooter _inst;
        [SerializeField] private bool _work;
        [SerializeField] private BootSettings _bootSettings;
        [SerializeField] private ObjectPoolsManager _poolsManager;   
        
        private void Awake()
        {
            if (_inst != null && _inst != this)
            {
                DestroyImmediate(gameObject);
                return;
            }
            _inst = this;
        }

        private void Start()
        {
            if (!_work 
                || !GlobalState.DevSceneMode 
                || GlobalState.SingleModeInitiated)
            {
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            GlobalState.SingleModeInitiated = true;
            GameManager.SetUSCulture();
            if (_bootSettings.CapFPS)
                Application.targetFrameRate = _bootSettings.FpsCap;
            var locator = gameObject.GetComponent<IGConLocator>();
            locator.InitContainer();
            var dataInit = gameObject.GetComponent<ISaveInitializer>();
            dataInit.InitSavedData();
            _poolsManager.BuildPools();
        }

        private LevelSpawner GetLevelSpawner()
        {
            return gameObject.GetComponent<LevelSpawner>();
        }

        private void SpawnLevel()
        {
            var ls = GetLevelSpawner();
            if (ls == null)
                return;
            ls.SpawnLevel();
        }

        private void DisableLevelAutoSpawn()
        {
            var ls = GetLevelSpawner();
            if (ls == null)
                return;
            ls.autoSpawn = false;
        }
    }
}