using System;
using System.Collections;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerGridTester : MonoBehaviour
    {
        public float delay = .25f;
        public Vector2Int gridSize;
        public GameObject testBlockPrefab;
        [Space(10)] 
        public bool giveToTower = true;
        public TowerBlocksBuilder blocksBuilder;
        public Tower tower;
        private bool _inited;
        
        [ContextMenu("Init")]
        public void Init()
        {
            CLog.Log("Init");
            _inited = true;
            // blocksBuilder.InitForArea(0, gridSize);
        }

        [ContextMenu("Build")]
        public void Build()
        {
            if(giveToTower)
                BuildTower();
            else
            {
                throw new Exception("sending to perimeter not implemented");
            }
        }
        

        public void BuildTower()
        {
            if (tower == null)
            {
                tower = FindObjectOfType<Tower>();
                if (tower == null)
                {
                    Debug.LogError($"No tower given or found");
                    return;
                }
            }
            StartCoroutine(GivingToTower());
        }

        private IEnumerator GivingToTower()
        {
            var count = gridSize.x * gridSize.y;
            var level = tower.Level + 1;
            if(level >= 5)
                yield break;
            var grid = tower.Team.TowerSettings.levelSettings[level].buildingSettings.gridSize;
            count = grid.x * grid.y;
            for (var i = 0; i < count && tower.CanTake(); i++)
            {
                var instance = Instantiate(testBlockPrefab);
                instance.transform.position = new Vector3(10, 1, 2);
                instance.gameObject.name = $"b_{i + 1}";
                var raft = instance.GetComponent<BoatPart>();
                if (raft == null)
                {
                    Debug.LogError($"No raft script");
                    yield break;
                }
                tower.TakeBoatPart(raft);
                yield return new WaitForSeconds(delay);
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                Build();
            }
        }
    }
}