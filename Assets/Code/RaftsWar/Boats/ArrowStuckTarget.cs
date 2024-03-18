using System.Collections.Generic;
using SleepDev;
using UnityEngine;

namespace RaftsWar.Boats
{
    public class ArrowStuckTarget : IArrowStuckTarget
    {
        private Transform _parent;
        private List<IArrow> _stuckArrows;
        
        public ArrowStuckTarget(Transform parent)
        {
            _stuckArrows = new List<IArrow>(50);
            _parent = parent;
        }

        public void StuckArrow(IArrow arrow)
        {
            // CLog.LogRed($"[ArrowsStash] ADDED ARROW {_stuckArrows.Count}");
            arrow.Go.transform.parent = _parent;
            _stuckArrows.Add(arrow);
        }

        public void HideAll()
        {
            // CLog.LogRed($"[ArrowsStash] stuck count to hide {_stuckArrows.Count}");
            foreach (var arrow in _stuckArrows)
                arrow.Hide();
            _stuckArrows.Clear();
        }
        
    }
}