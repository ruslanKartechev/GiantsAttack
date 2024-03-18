using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class SquareGrid
    {
        private Vector3 cellSize;
        private int width;
        private int height;
        private List<PosData> _gridPositions;
        private PosData[,] _grid2D;
        private int _ind_g;
        private int _indexX;
        private int _indexY;
        
        private int _perimeter;
        private int _area;
        private int _floor = 0;
        private float _yOffset;

        public int Width => width;
        public int Height => height;
       
        public int Area => _area;
        public int Floor => _floor;
        public int Perimeter => _perimeter;
        public int Ind_g => _ind_g;
        public Vector3 Center { get; set; } = new Vector3();
        public List<PosData> AllPositions => _gridPositions;
        public PosData[,] Grid2D => _grid2D;
        public Square WorldSquare { get; private set; }
        
        public SquareGrid(Vector3 cellSize, Vector2Int sideLength, float yOffset)
        {
            this.cellSize = cellSize;
            width = sideLength.x;
            height = sideLength.y;
            _yOffset = yOffset;
            // BuildGrid();
        }
        
        public void BuildGrid(Transform root)
        {
            _area = width * height;
            _grid2D = new PosData[width,height];
            _gridPositions = new List<PosData>(_area);
            var offset_x = width / 2f - .5f;
            var offset_z = height / 2f - .5f;
            var corner = new Vector3(-offset_x * cellSize.x,
                _yOffset,
                offset_z * cellSize.z);
            corner += Center;
            var lastZ = height - 1;
            var lastX = width - 1;
            
            var topLeftCorner = Vector3.zero;
            var topRightCorner = Vector3.zero;
            var botLeftCorner = Vector3.zero;
            var botRightCorner = Vector3.zero;
            
            for (var z = 0; z <= lastZ; z++)
            {
                for (var x = 0; x <= lastX; x++)
                {
                    var pos = corner;
                    pos.x += x * cellSize.x;
                    pos.z -= z * cellSize.z;
                    var data = new PosData(pos, ESquareSide.TopLeft);
                    if (z == 0) // top row
                    {
                        if (x == 0)
                        {
                            data.side = ESquareSide.TopLeft;
                            topLeftCorner = pos;
                        }
                        else if (x == lastX)
                        {
                            data.side = ESquareSide.TopRight;
                            topRightCorner = pos;
                        }
                        else
                            data.side = ESquareSide.Top;
                    }
                    else if (z == lastZ) // last row
                    {
                        if (x == 0)
                        {
                            data.side = ESquareSide.BotLeft;
                            botLeftCorner = new Vector3(pos.x, pos.y, pos.z);
                        }
                        else if (x == lastX)
                        {
                            data.side = ESquareSide.BotRight;
                            botRightCorner = pos;
                        }
                        else
                            data.side = ESquareSide.Bot;
                    }
                    else // rows in between
                    {
                        if (x == 0)
                            data.side = ESquareSide.Left;
                        else if (x == lastX)
                            data.side = ESquareSide.Right;
                        else
                            data.side = ESquareSide.None;
                    }
                    _gridPositions.Add(data);
                    _grid2D[x,z] = data;
                }
            }

            topLeftCorner += new Vector3(-.5f * cellSize.x, 0f,  .5f * cellSize.z);
            topRightCorner += new Vector3(.5f * cellSize.x, 0f,  .5f * cellSize.z);
            botLeftCorner += new Vector3(-.5f * cellSize.x, 0f,  -.5f * cellSize.z);
            botRightCorner += new Vector3(.5f * cellSize.x, 0f,  -.5f * cellSize.z);
            
            topLeftCorner = root.TransformPoint(topLeftCorner);
            topRightCorner = root.TransformPoint(topRightCorner);
            botLeftCorner = root.TransformPoint(botLeftCorner);
            botRightCorner = root.TransformPoint(botRightCorner);
            WorldSquare = new Square(topLeftCorner, topRightCorner, botLeftCorner, botRightCorner);
        }

        public PosData GetGridPos(int index)
        {
            var pp = _gridPositions[index];
            pp.position.y += _floor * cellSize.y;
            return pp;
        }
        
        public PosData GetNextGridPos()
        {
            if (_ind_g >= _area)
            {
                _floor++;
                _ind_g = 0;
            }
            var pp = _gridPositions[_ind_g];
            _ind_g++;
            pp.position.y += _floor * cellSize.y;
            return pp;
        }
        
        public void GetNextAreaPos(out PosData data, out int outInd)
        {
            var x = _indexX;
            var index = _indexY * width + x;
            outInd = index;
            _indexX++;
            if (_indexX >= width)
            {
                _indexX = 0;
                _indexY++;
            }
            data = _gridPositions[index];
        }
        
        public void GetNextAreaPosInverse(out PosData data, out int outInd)
        {
            var x = width - _indexX - 1;
            var index = _indexY * width + x;
            outInd = index;
            _indexX++;
            if (_indexX >= width)
            {
                _indexX = 0;
                _indexY++;
            }
            data = _gridPositions[index];
        }

        public void ZeroFloor()
        {
            _floor = 0;
        }

        public void ZeroIndices()
        {
            _ind_g = 0;
        }

        public void OffsetCenter(float xoffset, float zoffset)
        {
            var x = xoffset * cellSize.x;
            var z = zoffset * cellSize.z;
            Center = new Vector3(x, 0, z);
            // CLog.LogRed($"[*******] CENTER {Center}");
        }
        
    
        public class PosData
        {
            public PosData(Vector3 position, ESquareSide side)
            {
                this.position = position;
                this.side = side;
            }

            public Vector3 position;
            public ESquareSide side;
        }
    }
    
}