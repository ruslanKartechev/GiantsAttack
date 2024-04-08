using UnityEngine;

namespace GiantsAttack
{
    public class SplineEventBreakBuilding : SplineEvent
    {
        [SerializeField] private BrokenBuilding _building;

 
        public override void Activate()
        {
            _building.Break();
        }
    }
}