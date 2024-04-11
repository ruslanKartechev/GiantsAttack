using SleepDev.Pooling;
using GiantsAttack;

namespace GameCore.Core
{
    public interface IObjectPoolsManager
    {
        void BuildPools();
        void RecollectAll();
        
        public IObjectPool<IBullet> BulletsPool { get;  }
        public IObjectPool<BulletCasing> BulletCasingsPool { get; }
    }
}