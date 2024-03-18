using SleepDev.Pooling;

namespace RaftsWar.Boats
{
    public class PooledCatapultProjectile : CatapultProjectile, IPooledObject<ICatapultProjectile>
    {
        public IObjectPool<ICatapultProjectile> Pool { get; set; }
        public ICatapultProjectile Target => this;
     
        public void Destroy()
        {
            Destroy(gameObject);
        }
        
        public override void Hide()
        {
            base.Hide();
            Pool.ReturnObject(this);
        }
    }
}