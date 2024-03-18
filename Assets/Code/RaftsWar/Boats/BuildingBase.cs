using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class BuildingBase : MonoExtended
    {
        [SerializeField] protected Transform _root;
        private List<IBuildingBlock> _available = new List<IBuildingBlock>();

        public AreaBlock AreaBlock { get; set; }

        public void Clear()
        {
            // if (AreaBlock == null)
            //     return;
        }

        public void Init(TowerRaftSettings settings)
        {
            AreaBlock = new AreaBlock(settings.gridSize);

        }
        
        public void SpawnTransparent(SquareGrid grid, Vector2Int gridSize, float blockScale, float yScale)
        {
            AreaBlock = new AreaBlock(gridSize);
            var count = grid.Area;
            var addToSpawn = count - _available.Count;
            while (addToSpawn > 0)
            {
                var instance = Spawn();
                instance.transform.parent = _root;
                _available.Add(instance);
                addToSpawn--;
            }
            var ind = 0;
            for (ind = 0; ind < count; ind++)
            {
                var pp = grid.GetGridPos(ind);
                var instance = _available[ind];
                instance.Show();
                instance.Transform.localPosition = pp.position;
                instance.SetSide(pp.side);
                instance.SetScale(blockScale);
                instance.SetYScale(yScale);
                AreaBlock.AddNext(instance);
            }
            var countToHide = _available.Count - count;
            while (countToHide > 0)
            {
                _available[ind].Hide();
                ind++;
                countToHide--;
            }

            BuildingBlock_RaftCell Spawn()
            {
                if(Application.isPlaying)
                    return GCon.GOFactory.Spawn<BuildingBlock_RaftCell>(GlobalConfig.BuildingCellID);
#if UNITY_EDITOR
                    return GCon.GOFactory.SpawnAsPrefab(GlobalConfig.BuildingCellID).GetComponent<BuildingBlock_RaftCell>();
#else
                    return GCon.GOFactory.Spawn<BuildingBlock_RaftCell>(GlobalConfig.BuildingCellID);
#endif
            }
        }
        

        public void HideAt(int index)
        {
            
            AreaBlock.GetAt(index).Hide();
        }
    }
}