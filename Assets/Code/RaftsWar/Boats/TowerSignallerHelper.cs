using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class TowerSignallerHelper
    {
              
        public static List<Vector3> directions = new()
        {
            new(0, 0, -1),
            // new(-1, 0, -1),
            new(-1, 0, 0),
            // new(-1, 0, 1),
            new(0, 0, 1),
            // new(1, 0, 1),
            new(1, 0, 0),
            // new(1, 0, -1),
        };

        public int _directionIndex = 0;

        public int GetInd()
        {
            var ind = _directionIndex;
            _directionIndex++;
            if(_directionIndex >= directions.Count)
                _directionIndex = 0;
            return ind;
        }
    }
}