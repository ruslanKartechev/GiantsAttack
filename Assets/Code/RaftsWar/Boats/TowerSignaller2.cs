using System.Collections;
using System.Collections.Generic;
using SleepDev;
using UnityEditor;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerSignaller2 : MonoExtended
    {
        [System.Serializable]
        public class TowerData
        {
            public Tower tower;
            public List<Transform> spawnPointGroups;
        }

        public int towerLevels = 4;
        public float partsGivingDelay = .5f;
        public float nextTowerDelay = .5f;
        public float nextLevelDelay = .1f;
        public float startDelay = .2f;
        public float spawnWidth = 5f;
        public float spawnHeight = 5f;
        [Space(10)] 
        [SerializeField] private CameraRailMover _railMover;
        [SerializeField] private Team _team;
        [SerializeField] private List<TowerData> _towers;
        
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private GlobalConfig _globalConfig;
        private TowerSignallerHelper _helper = new TowerSignallerHelper();
        private bool _inited;

        private void Start()
        {
            Delay(() =>
            {
                Init();
                Begin();
            }, startDelay);
        }

        public void Init()
        {
            CLog.Log($"[{nameof(TowerSignaller2)}] Init");
            _inited = true;
            _globalConfig.SetupStaticFields();
            var teamsTargetsManager = new TeamsTargetsManager();
            foreach (var tower in _towers)
                tower.tower.Init(_team);
        }
        
        /// <summary>
        /// For timeline
        /// </summary>
        [ContextMenu("Begin")]
        public void Begin()
        {
            CLog.Log($"[{nameof(TowerSignaller2)}] Begin");
            if (!_inited)
            {
                CLog.Log($"[{nameof(TowerSignaller2)}] not inited...");
                return;
            }
            StartCoroutine(Giving());
            _railMover.Move();
        }
        
        private IEnumerator Giving()
        {
            for (var towerI = 0; towerI < _towers.Count; towerI++)
            {
                var tower = _towers[towerI].tower;
                tower.gameObject.SetActive(true);
                CLog.LogGreen($"Tower index {towerI}");
                for (var levelI = 0; levelI < towerLevels; levelI++)
                {
                    CLog.LogBlue($"Tower level {towerI}");
                    if (towerI > 0)
                    {
                        _towers[towerI-1].tower.E_HideBuildingArea(levelI);
                    }
                    for (var partI = 0; partI < 4; partI++)
                    {
                        while (tower.CanTake() == false)
                            yield return null;
                        var newPart = Spawn();
                        tower.TakeBoatPart(newPart);
                        yield return new WaitForSeconds(partsGivingDelay);
                    }
                    if (towerI > 0)
                    {
                        yield return new WaitForSeconds(nextLevelDelay);
                        var prevTower = _towers[towerI - 1];
                        prevTower.tower.E_HideViewLevel(levelI);
                        prevTower.spawnPointGroups[levelI].gameObject.SetActive(false);
                    }
                }
                yield return new WaitForSeconds(nextTowerDelay);
            }
        }

        private BoatPart Spawn()
        {
            var bp = GCon.GOFactory.Spawn<BoatPart>(GlobalConfig.BoatPartID);
            bp.ColliderOff();
            var spawnPos = _spawnPoint.position;
            spawnPos.x += UnityEngine.Random.Range(-spawnWidth, spawnWidth);
            spawnPos.z += UnityEngine.Random.Range(-spawnHeight, spawnHeight);
            bp.transform.position = spawnPos;
            return bp;
        }
    }
    
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(TowerSignaller2))]
    public class TowerSignaller2Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var me = target as TowerSignaller2;
            GUILayout.Space(20);
            const float width = 90;
            if (GUILayout.Button("Init", GUILayout.Width(width)))
            {
                me.Init();
            }
            
            if (GUILayout.Button("Begin", GUILayout.Width(width)))
            {
                me.Begin();
            }
        }
    }
    #endif
}