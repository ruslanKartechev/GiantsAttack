using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class UnloadPointsManager : IUnloadPointsManager
    {
        private List<Vector3> _points = new List<Vector3>();
        private ITowerBlocksBuilder _builder;
        private float _offset;
        
        public UnloadPointsManager(ITowerBlocksBuilder builder, float offset)
        {
            _builder = builder;
            _offset = offset;
        }
        
        public void AddLatestPoints()
        {
            AddForGrid(_builder.LatestGrid);
        }

        public void AddToAllLevels()
        {
            foreach (var grid in _builder.Grids)
                AddForGrid(grid);
        }

        private void AddForGrid(SquareGrid grid)
        {
            var square = grid.WorldSquare;
            AddCorner(square.TopLeftCorner);
            AddCorner(square.TopRightCorner);
            AddCorner(square.BotLeftCorner);
            AddCorner(square.BotRightCorner);
        
            void AddCorner(Vector3 corner)
            {
                var offset = corner - square.Center;
                var magn = offset.magnitude;
                offset /= magn;
                magn += _offset;
                var pos = square.Center + offset * magn;
                _points.Add(pos);
                #if UNITY_EDITOR
                Debug.DrawLine(square.Center, pos, Color.magenta, 20f);
                #endif
            }
        }
        
        public Vector3 GetClosestPoint(Vector3 closestTo)
        {
            var closest = Vector3.zero;
            var mind2 = float.MaxValue;
            foreach (var point in _points)
            {
                // Debug.DrawLine(point + Vector3.up, closestTo + Vector3.up, Color.black, 3f);
                var vec = closestTo - point;
                var d2 = vec.XZDistance2();
                if (d2 <= mind2)
                {
                    mind2 = d2;
                    closest = point;
                }
            }
            return closest;

        }
    }
}