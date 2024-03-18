using System;
using System.Collections.Generic;
using DG.Tweening;
using SleepDev;
using UnityEngine;
using UnityEngine.AI;

namespace RaftsWar.Boats
{
    public class TowerBlocksBuilder : MonoExtended, ITowerBlocksBuilder
    {
        public event Action OnCanUpgrade;
        
        public float moveTime;
        public Ease moveEase;
        [Space(10)]
        [SerializeField] private Vector3 _cellSize;
        [SerializeField] private float _blockScale = 1f;
        [Space(10)]
        [SerializeField] private Transform _root;
        [SerializeField] private BuildingBase _buildingArea;
        private SquareGrid _latestGrid;
        private List<SquareGrid> _grids = new List<SquareGrid>(4);
        private List<AreaBlock> _areaBlocks = new List<AreaBlock>(4);
        private List<NavMeshObstacle> _obstacles = new List<NavMeshObstacle>(100);
        private int _storedCount;
        private int _areaCount;
        private bool _canAccept;
        private int _level;

        public TowerRaftSettings BuildingSettings { get; set; }
        public SquareGrid LatestGrid => _latestGrid;
        public List<SquareGrid> Grids => _grids;
        public int StoredCount => _storedCount;
        public bool CanAccept => _canAccept;
        public int Stored => _storedCount;
        public int Area => _areaCount;
        public Transform Root => _root;
        public Vector3 CellSize => _cellSize;
        public AreaBlock CurrentArea => _areaBlocks[_level];

        public float BlockScale
        {
            get => _blockScale;
            set => _blockScale =value;
        }
        
        private float YScale => BuildingSettings.yScale;
        private float FloorPosY => BuildingSettings.floorYPos;

        public void BuildArea(TowerRaftSettings buildingSettings, int level)
        {
            _level = level;
            BuildingSettings = buildingSettings;
            var size = buildingSettings.gridSize;
            _areaCount = size.x * size.y;
            ClearBlocks();
            BuildNewBuildingArea(size);
            _grids.Add(LatestGrid);
            _canAccept = true;
        }


        public void BuildUpToLevel(Team team, int level)
        {
            var settingsList = team.TowerSettings.levelSettings;
            _grids = new List<SquareGrid>(level + 1);
            for (var i = 0; i < level; i++)
                _grids.Add(SpawnFilledGrid(i));
            
            if (level == 4) // lastPossible level
            {
                _latestGrid = SpawnFilledGrid(level);
                _grids.Add(_latestGrid);
                _canAccept = false;
            }
            else // not last level, so spawn transparent grid
            {
                BuildingSettings = settingsList[level].buildingSettings;
                _latestGrid = BuildGrid(BuildingSettings.gridSize, level);
                _grids.Add(_latestGrid);
                _buildingArea.Init(BuildingSettings);
                _buildingArea.SpawnTransparent(_latestGrid,  BuildingSettings.gridSize, BlockScale, YScale);
                CreateAreaBlock(BuildingSettings.gridSize);
                _canAccept = true;
            }
            _areaCount = BuildingSettings.gridSize.x * BuildingSettings.gridSize.y;
            _level = level;
            UpdateCentralArea();
            
            // Internal methods //
            SquareGrid SpawnFilledGrid(int levelInd)
            {
                BuildingSettings = settingsList[levelInd].buildingSettings;
                var grid = BuildGrid(BuildingSettings.gridSize, levelInd);
                var areaBlock = CreateAreaBlock(BuildingSettings.gridSize);
                SpawnCells(grid, GlobalConfig.BoatPartID, areaBlock, team.BoatView);
                return grid;
            }
        }
        
        private void SpawnCells(SquareGrid grid, string id, AreaBlock areaBlock, BoatViewSettings boatView)
        {
            for(var c = 0; c < grid.Area; c++)
            {
                var posData = grid.GetGridPos(c);
                var cell = GetBlock(id);
                cell.Take();
                cell.Transform.parent = _root;
                cell.Transform.localPosition = posData.position;
                cell.SetScale(BlockScale);
                cell.SetYScale(BuildingSettings.yScale);
                cell.SetSide(posData.side);
                cell.Transform.GetComponent<BoatPart>().SetView(boatView);
                areaBlock.AddNext(cell);
            }
        }

        private IBuildingBlock GetBlock(string id)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return GCon.GOFactory.Spawn(id).GetComponent<IBuildingBlock>();
            return GCon.GOFactory.SpawnAsPrefab(id).GetComponent<IBuildingBlock>();
#endif
            return GCon.GOFactory.Spawn(id).GetComponent<IBuildingBlock>();
        }
        
        public void Destroy()
        {
            DestroyBlock(_buildingArea.AreaBlock);
            foreach (var block in _areaBlocks)
                DestroyBlock(block);
            void DestroyBlock(AreaBlock block)
            {
                for (var z = 0; z < block.Blocks.GetLength(1); z++)
                {
                    for (var x = 0; x < block.Blocks.GetLength(0); x++)
                    {
                        if (block.Blocks[x, z] == null)
                            return;
                        block.Blocks[x,z].Destroy();
                    }
                }
            }
        }

        public void HideAtLevel(int level)
        {
            _areaBlocks[level].HideAll();
        }

        public void TakeBuildingBlock(IBuildingBlock bb)
        {
            if (_canAccept == false)
            {
                CLog.LogRed($"[GridManager] Cannot accept");
                return;
            }
            CurrentArea.AddNext(bb);
            bb.Take();
            SquareGrid.PosData pp = null;
            var index = 0;
            if(_level == 2)
                _latestGrid.GetNextAreaPosInverse(out pp, out index);
            else
                _latestGrid.GetNextAreaPos(out pp, out index);
            bb.SetScale(BlockScale);
            bb.SetYScale(YScale);
            bb.SetSide(pp.side);
            SendRaft(bb.Transform, pp.position, index);
            AddNavMeshObstacle(pp.position);
            _storedCount++;
            if (_storedCount == _areaCount)
                CallCanUpgrade();
        }

        private void AddNavMeshObstacle(Vector3 localPos)
        {
            var go = new GameObject("tno");
            go.transform.parent = _root;
            go.transform.localPosition = localPos;
            var obstacle = go.AddComponent<NavMeshObstacle>();
            obstacle.size = _cellSize * BlockScale;
            _obstacles.Add(obstacle);
        }
        
        public void SendRaft(Transform tr, Vector3 localPos, int cellIndex)
        {
            tr.parent = _root;
            var ss = DOTween.Sequence();
            var p1 = tr.localPosition + Vector3.up * 5f;
            ss.Append(tr.DOLocalMove(p1, moveTime / 2f).SetEase(moveEase));
            ss.Append(tr.DOLocalMove(localPos, moveTime).SetEase(moveEase));
            ss.OnComplete(() =>
            {
                _buildingArea.HideAt(cellIndex);
                tr.localScale = Vector3.one;
            });
        }

        private void CallCanUpgrade()
        {
            // CLog.LogRed("[TowerBuilder] Calling to upgrade ");
            _canAccept = false;
            OnCanUpgrade?.Invoke();
        }
        
        /// <summary>
        /// Calls Building Area to spawn NEW transparent cells to fuill
        /// </summary>
        private void SpawnBuildingArea()
        {
            _buildingArea.Clear();
            _buildingArea.SpawnTransparent(_latestGrid, BuildingSettings.gridSize, BlockScale, YScale);
        }

        private void UpdateCentralArea()
        {
            if (_level == 0)
                return;
            var grid = _grids[0];
            ModifyCentralGridByTowerLevel(grid, _level);
            _areaBlocks[0].UpdateSidesToGrid(grid);
        }

        /// <summary>
        /// Builds a grid for the NEW tower building area. Modifies by tower level
        /// </summary>
        public SquareGrid BuildGrid(Vector2Int size, int level)
        {
            var grid = new SquareGrid(_cellSize * BlockScale, size, FloorPosY);
            switch (level)
            {
                case 1:
                    grid.OffsetCenter(2,0);        
                    break;  
                case 2:
                    grid.OffsetCenter(-2,0);
                    break;  
                case 3:
                    grid.OffsetCenter(0,-2);
                    break;
            }
            grid.BuildGrid(_root);
            ModifyGridByTowerLevel(grid, level);
            return grid;
        }
        
        private AreaBlock CreateAreaBlock(Vector2Int size)
        {
            var areaBlock = new AreaBlock(size);
            _areaBlocks.Add(areaBlock);
            return areaBlock;
        }
        
        private void BuildNewBuildingArea(Vector2Int size)
        {
            _latestGrid = BuildGrid(size, _level);
            _grids.Add(_latestGrid);
            CreateAreaBlock(size);
            SpawnBuildingArea();
            UpdateCentralArea();
        }
        private void ClearBlocks()
        {
            _storedCount = 0;
        }
        
        
        
        
        
        
        
        
        
        public static void ModifyCentralGridByTowerLevel(SquareGrid grid, int level)
        {
            switch (level)
            {
                case 0:
                    break;
                case 1: // hide on the right
                    ModifyHideRight(grid);
                    break;
                case 2: // hide on the right and left
                    ModifyHideRight(grid);
                    ModifyHideLeft(grid);
                    break;
                case 3: // hide on the bottom
                    ModifyHideRight(grid);
                    ModifyHideLeft(grid);
                    var count = grid.Width-1;
                    var y = grid.Height - 1;
                    for (var ix = 0; ix <= count; ix++)
                        grid.Grid2D[ix, y].side = ESquareSide.None;
                    break;
                case 4: break;
            }
        }

        
        public static void ModifyGridByTowerLevel(SquareGrid grid, int level)
        {
            var count = 0;
            switch (level)
            {
                case 0:
                    break;
                case 1: // hide on the left
                    ModifyHideLeft(grid);
                    break;
                case 2: // hide on the right
                    ModifyHideRight(grid);
                    break;
                case 3: // hide on the top
                    ModifyHideTop(grid);
                    break;
                case 4:
                    break;
            }
        }
        
        
        private static void ModifyHideLeft(SquareGrid grid)
        {
            var count = grid.Height-1;
            for (var iy = 0; iy <= count; iy++)
            {
                if(iy == 0)
                    grid.Grid2D[0, iy].side = ESquareSide.Top;
                else if (iy == count)
                    grid.Grid2D[0, iy].side = ESquareSide.Bot;
                else
                    grid.Grid2D[0, iy].side = ESquareSide.None;
            }
        }
        
        private static void ModifyHideRight(SquareGrid grid)
        {
            var count = grid.Height;
            var x = grid.Width - 1;
            for (var i = 0; i < count; i++)
            {
                count = grid.Height-1;
                for (var iy = 0; iy <= count; iy++)
                {
                    if(iy == 0)
                        grid.Grid2D[x, iy].side = ESquareSide.Top;
                    else if (iy == count)
                        grid.Grid2D[x, iy].side = ESquareSide.Bot;
                    else
                        grid.Grid2D[x, iy].side = ESquareSide.None;
                }
            }
        }
        
        private static void ModifyHideTop(SquareGrid grid)
        {
            var count = grid.Width-1;
            for (var ix = 0; ix <= count; ix++)
            {
                if(ix == 0)
                    grid.Grid2D[ix, 0].side = ESquareSide.Left;
                else if (ix == count)
                    grid.Grid2D[ix, 0].side = ESquareSide.Right;
                else
                    grid.Grid2D[ix, 0].side = ESquareSide.None;
            }
        }
        
    }
}