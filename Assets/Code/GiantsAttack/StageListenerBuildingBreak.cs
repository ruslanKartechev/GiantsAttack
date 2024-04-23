using UnityEngine;

namespace GiantsAttack
{
    public class StageListenerBuildingBreak : StageListener
    {
        [SerializeField] private BrokenBuilding _building;
        
        public override void OnActivated()
        {
            _building.Break();
        }

        public override void OnStopped()
        {
        }

        public override void OnCompleted()
        {
        }
    }
}