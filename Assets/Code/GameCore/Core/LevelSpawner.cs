using SleepDev;
using UnityEngine;

namespace GameCore.Core
{
    [DefaultExecutionOrder(1000)]
    public class LevelSpawner : MonoBehaviour
    {
        public byte environmentIndex;
        public bool usePreloaded;
        public GameObject preloaded;
        public bool findPreloaded;
        [Header("Main")]
        public bool autoSpawn = true;
        public bool initLevel = true;
        [Header("SpawnPoints")]
        public Transform defaultPosition;
        public Light globalLight;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (findPreloaded && preloaded == null)
            {
                var ll  = FindObjectOfType<Level>();
                if (ll != null)
                    preloaded = ll.gameObject;
            }
            if (globalLight == null)
            {
                var ll = FindObjectOfType<Light>();
                if (ll != null && ll.gameObject.name.Contains("Directional"))
                {
                    globalLight = ll;
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }
#endif

        private void Start()
        {
#if !UNITY_EDITOR
            autoSpawn = true;
            usePreloaded = findPreloaded = false;
#endif
            GCon.UIFactory.Clear();
            EnvironmentState.CurrentIndex = environmentIndex;
            EnvironmentState.CurrentGlobalLight = globalLight;
            if (!autoSpawn)
                return;
            SpawnLevel();
        }

        public void SpawnLevel()
        {
            if (GlobalState.DevSceneMode)
            {
                var levelManager = (LevelManager)GCon.LevelManager;
                levelManager.SetupLevels();
            }
            if (usePreloaded)
            {
                Level pl = null;
                if (preloaded == null)
                    pl = FindObjectOfType<Level>();
                else
                    pl = preloaded.GetComponent<Level>();
                if (pl != null)
                {
                    CLog.Log($"[LevelSpawner] Init preloaded level");
                    pl.Init();
                    return;
                }
                CLog.Log($"[LevelSpawner] NO preloaded level found");
            }

            if (GlobalState.DevSceneMode)
                SpawnLevel(GCon.PlayerData.LevelTotal);
            else
                SpawnLevelFromPrefab(GCon.LevelManager.CurrentLevel.Prefab());
        }

        private GameObject GetPrefab(ILevelData levelData)
        {
            var loc = $"Prefabs/Levels/{levelData.LevelName}";
            CLog.Log($"[LevelSpawner] Getting prefab at {loc}");
            var prefab = Resources.Load<GameObject>(loc);
#if UNITY_EDITOR
            if (levelData.LevelName.Contains("_c_"))
            {
                CLog.LogGreen($"Loading a creative level");
                prefab = Resources.Load<GameObject>($"Prefabs/Levels/Creatives/{levelData.LevelName}");
            }
#endif
            return prefab;
        }

        private void SpawnLevel(int index)
        {
            var levelData = GCon.LevelRepository.GetLevel(index);
            CLog.Log($"[LevelSpawner] Spawning: {index}, {levelData.LevelName}, scene {levelData.SceneName}");
            SpawnLevelFromPrefab(GetPrefab(levelData));
        }

        private void SpawnLevelFromPrefab(GameObject prefab)
        {
            CLog.Log($"[LevelSpawner] Spawning from prefab {prefab.name}");
            var spawnPoint = defaultPosition;
            var instance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            var level = instance.GetComponent<Level>();
            if (initLevel)
                level.Init();
        }

 
#if UNITY_EDITOR
        [Header("Editor Only")]
        public int e_levelIndex;
        public LevelsRepository e_levelrepo;
        
        public void E_Clear()
        {
            if(preloaded != null)
                DestroyImmediate(preloaded.gameObject);
        }

        public void E_Next()
        {
            E_Clear();
            e_levelIndex++;
            e_levelIndex = Mathf.Clamp(e_levelIndex, 0, e_levelrepo.Count - 1);
            E_SpawnLevel();
        }

        public void E_Prev()
        {
            E_Clear();
            e_levelIndex--;
            e_levelIndex = Mathf.Clamp(e_levelIndex, 0, e_levelrepo.Count - 1);
            E_SpawnLevel();
        }

        public void E_SpawnLevel()
        {
            var levelData = e_levelrepo.GetLevel(e_levelIndex);
            var prefab = GetPrefab(levelData);
            var spawnPoint = defaultPosition;
            preloaded = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            preloaded.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }
        #endif
    }
}