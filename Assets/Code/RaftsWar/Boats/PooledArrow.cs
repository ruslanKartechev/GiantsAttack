using SleepDev.Pooling;

namespace RaftsWar.Boats
{
    public class PooledArrow : Arrow,  IPooledObject<IArrow>
    {
        public IObjectPool<IArrow> Pool { get; set; }
        public IArrow Target => this;

        public override void Hide()
        {
            base.Hide();
            Pool.ReturnObject(this);
        }
    }
}