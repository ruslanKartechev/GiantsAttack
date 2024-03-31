using GameCore.UI;
using GiantsAttack;
using SleepDev;
using SleepDev.Levels;
using SleepDev.Pooling;
using SleepDev.Saving;
using SleepDev.Scenes;
using SleepDev.SlowMotion;

namespace GameCore.Core
{
    public static partial class GCon
    {
        public static IPlayerData PlayerData { get; set; }
        public static IDataSaver DataSaver { get; set; }
        public static ISceneSwitcher SceneSwitcher { get; set; }
        public static ILevelManager LevelManager { get; set; }
        public static ILevelRepository LevelRepository { get; set; }
        public static ISlowMotionManager SlowMotion { get; set; }
        public static IPlayerInput Input { get; set; }
        public static GlobalConfig GlobalConfig { get; set; }
        
        public static UISpawner UIFactory { get; set; }
        public static GameObjectFactory GOFactory { get; set; }
        
        public static IObjectPoolsManager PoolsManager { get; set; }
        public static IObjectPool<IBullet> BulletsPool { get; set; }
        public static IObjectPool<BulletCasing> BulletCasingsPool { get; set; }

    }
}
