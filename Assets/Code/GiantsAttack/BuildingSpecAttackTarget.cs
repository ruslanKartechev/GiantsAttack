using UnityEngine;

namespace GiantsAttack
{
    public class BuildingSpecAttackTarget : MonoBehaviour, ISpecAttackTarget
    {
        [SerializeField] private BrokenBuilding _building;

        public void OnStageBegan()
        {
        }

        public void OnAttackBegan()
        {
        }

        public void OnAttack()
        {
            _building.Break();
        }

        public void OnCompleted()
        {
        }
    }
}