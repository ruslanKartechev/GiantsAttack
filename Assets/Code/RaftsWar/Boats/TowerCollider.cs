using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RaftsWar.Boats
{
    [System.Serializable]
    public class TowerCollider : MonoBehaviour
    {
        private const float ColliderSizeMultiplier = 1.2f;
        [SerializeField] private TowerBlocksBuilder _builder;
        [SerializeField] private BoxCollider _blockerPrefab;
        [SerializeField] private BoxCollider _towerPrefab;
        private List<NavMeshObstacle> _obstacles;
        private List<Collider> _blockerColls;
        private List<Collider> _towerColls;

        public void Init()
        {
            _blockerColls = new List<Collider>(4);
            _towerColls = new List<Collider>(4);
            _obstacles = new List<NavMeshObstacle>(4);
        }
        
        public void On()
        {
            foreach (var c in _blockerColls)
                c.enabled = true;
            foreach (var c in _towerColls)
                c.enabled = true;
        }

        public void Off()
        {
            foreach (var c in _blockerColls)
                c.enabled = false;
            foreach (var c in _towerColls)
                c.enabled = false;
            foreach (var ob in _obstacles)
                ob.enabled = false;
        }

        public void UpdateCollider()
        {
            var grid = _builder.LatestGrid;
            var sizeBlocker = _builder.CellSize * (grid.Width * _builder.BlockScale);
            var sizeTower = sizeBlocker * ColliderSizeMultiplier;
            SpawnBlocker(sizeBlocker, grid.Center);
            SpawnTower(sizeTower, grid.Center);
        }

        public void SetupForAllLevels()
        {
            foreach (var grid in _builder.Grids)
            {
                var sizeBlocker = _builder.CellSize * (grid.Width * _builder.BlockScale);
                var sizeTower = sizeBlocker * ColliderSizeMultiplier;
                SpawnBlocker(sizeBlocker, grid.Center);
                SpawnTower(sizeTower, grid.Center);
            }
        }

        private void SpawnTower(Vector3 size, Vector3 center)
        {
            var coll = Instantiate(_towerPrefab, _builder.Root);
            coll.transform.localPosition = center;
            coll.size = size;
            _towerColls.Add(coll);
        }
        
        private void SpawnBlocker(Vector3 size, Vector3 center)
        {
            var coll = Instantiate(_blockerPrefab, _builder.Root);
            coll.transform.localPosition = center;
            coll.size = size;
            _blockerColls.Add(coll);
            var ob = coll.gameObject.GetComponent<NavMeshObstacle>();
            ob.size = new Vector3(size.x, size.y, size.z);
            ob.enabled = true;
            _obstacles.Add(ob);
        }
    }
}