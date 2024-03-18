using System.Collections.Generic;
using UnityEngine;
using SleepDev;

namespace RaftsWar.Boats
{
    public class AreaBlock
    {
        private IBuildingBlock[,] _blocks;
        private int _ySize;
        private int _xSize;
        private int _yInd;
        private int _xInd;
        
        public IBuildingBlock[,] Blocks => _blocks;

        public AreaBlock(Vector2Int size)
        {
            _blocks = new IBuildingBlock[size.x, size.y];
            _xSize = size.x;
            _ySize = size.y;
        }

        public void AddNext(IBuildingBlock block)
        {
            Blocks[_xInd, _yInd] = block;
            _xInd++;
            if (_xInd >= _xSize)
            {
                _xInd = 0;
                _yInd++;
            }
        }

        public void RefreshInd()
        {
            _yInd = 0;
            _xInd = 0;
        }

        public void UpdateSidesToGrid(SquareGrid grid)
        {
            for (var z = 0; z < grid.Grid2D.GetLength(1); z++)
            {
                for (var x = 0; x < grid.Grid2D.GetLength(1); x++)
                {
                    _blocks[x,z].SetSide(grid.Grid2D[x,z].side);
                }                
            }
        }
        

        public void HideAllRightSides()
        {
            var x = _xSize - 1;
            for (var i = 0; i < _ySize; i++)
            {
                _blocks[x,i].HideRampSide(ESquareSide.Right);
            }
        }
        
        public void HideAllLeftSides()
        {
            var x = 0;
            for (var i = 0; i < _ySize; i++)
            {
                _blocks[x,i].HideRampSide(ESquareSide.Left);
            }
        }
        
        public void HideAllTopSides()
        {
            var y = 0;
            for (var i = 0; i < _xSize; i++)
            {
                _blocks[i, y].HideRampSide(ESquareSide.Top);
            }
        }
        
        public void HideAllBottomSides()
        {
            var y = _ySize-1;
            for (var i = 0; i < _xSize; i++)
            {
                _blocks[i, y].HideRampSide(ESquareSide.Bot);
            }
        }

        public void HideAll()
        {
            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                {
                    _blocks[x,y].Hide();
                }                
            }
        }

        public IBuildingBlock GetAt(int index)
        {
            var y = index / _xSize;
            var x = index % _xSize;
            return _blocks[x, y];
        }

        public List<IBuildingBlock> GetAll()
        {
            var list = new List<IBuildingBlock>(_xSize * _ySize);
            for (var y = 0; y < _ySize; y++)
            {
                for (var x = 0; x < _xSize; x++)
                    list.Add(_blocks[x,y]);
            }
            return list;
        }
    }
}