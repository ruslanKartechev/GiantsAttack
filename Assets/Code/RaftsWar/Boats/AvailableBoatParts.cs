using System.Collections.Generic;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class AvailableBoatParts
    {
        private List<BoatPart> _pool = new List<BoatPart>(40);

        public List<BoatPart> Parts => _pool;
        
        public void Add(BoatPart part)
        {
#if UNITY_EDITOR
            if (_pool.Contains(part))
            {
                Debug.LogError($"[BoatPartsPool] ************* ERROR *********** ALREADY CONTAINS PART");
            }
#endif
            _pool.Add(part);
        }

        public void Remove(BoatPart part)
        {
            _pool.Remove(part);
        }

        public void Clear()
        {
            _pool.Clear();
        }

        public int Count => _pool.Count;

        public void SetParts(IList<BoatPart> parts)
        {
            foreach (var pp in parts)
                _pool.Add(pp);
        }
    }
}