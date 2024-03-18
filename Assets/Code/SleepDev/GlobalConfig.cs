using UnityEngine;
using RaftsWar.Boats;

namespace SleepDev
{
    [CreateAssetMenu(menuName = "SO/GlobalConfig", fileName = "GlobalConfig", order = 0)]
    public class GlobalConfig : ScriptableObject
    {
        public const string BoatPartTag = "BoatPart";
        public const string ReceiverTag = "Tower";
        public const string ObstacleTag = "Obstacle";
        
        public const string ArrowID = "arrow";
        public const string ArcherUnitID = "unit_archer";
        public const string EnemyBoatID = "boat_enemy";
        public const string PlayerBoatID = "boat_player";
        public const string TowerID = "tower";
        public const string BoatPartBrokenID = "boatpart_broken";
        public const string CatapultID = "catapult";
        public const string CatapultViewID = "catapult_view";
        public const string BuildingCellID = "building_cell";
        public const string BoatPartID = "boat_part";
        public const string BoatPartWithUnitID = "boat_part_unit";
        public const string CatapultProjectileID = "catapult_projectile";

        public const float TowerProgressFillTime = .2f;
        public const float HealthFillTime = .15f;

        public const int DefaultLayer = 0;
        public const int BlockedLayer = 6;
        public const int DamageableLayer = 7;
        public const int BoatLayer = 8;

        public static float CaptainRotationLerp = .1f;
        public static float ArrowSpeed = 5f;
        public static float CatapultProjectileInflection = 5f;

        public static float SinkingDelay = 2;

        public static float TowerSpawningDuration = 1f;
        public static float TowerSpawningDelay = .25f;
        public static float ToTowerCameraMoveTime = 0.5f;
        public static float ToTowerCameraWait = 2f;

        public static float PlayerCameraSetTime = 1f;
        public static float PlayerCameraReturnTime = 1f;
        public static float BoatPartConnectTime = .1f;
        public static float BoatBreakForce = 20f;
        public static float UnitRagdollForce = 20f;
        public static float TowerUnloadPointsOffset = 4f;
        public static float PlayerFullIndicatorDuration = 3f;
        public static int PlayerBoatMaxConnectionsCount = 8;
        public static float BombDamage = 10f;
        public static float CollisionDamageMultiplier = 2f;
        public static float RadiusChangeTime = .2f;
        public static float TowerUnitsRotTime = .1f;
        public static int CastSkippedFrames = 1;

        public static BoatPushSettings BoatPushSettings;

        public static LayerMask BlockMask;        
        public static LayerMask DamageableMask;
        /// <summary>
        /// Debugging
        /// </summary>
        public static bool TowersNoAttack;
        public static bool EnemyNotActivated;
        public static bool BoatUnitsNoAttack;

        [Header("Options for Debug")] 
        public bool towersNoAttack;
        public bool enemyNotActivated;
        public bool boatUnitsNoAttack;
        [Header("Controls")]
        public float joystickSensitivity;
        public float joystickRad;
        public float captainRotationLerp = .1f;
        [Header("Global variables")]
        public float arrowSpeed = 3f;
        public float boatBreakForce = 20;
        public float unitRagdollForce = 20;
        public float boatPartConnectTime = .1f;
        public float playerCameraSetTime = 1f;
        public float playerCameraReturnTime = .5f;
        public float towerUnloadPointsOffset = 4f;
        public float playerFullIndicatorDuration = 3f;
        public int playerBoatMaxConnectionsCount =8;
        public float bombDamage = 10f;
        public float collisionDamageMultiplier = 2f;
        public float radiusChangeTime = .2f;
        public float towerUnitsRotTime = .1f;
        public int castSkippedFrames = 1;
        [Header("Tower")]
        public float towerSpawningDuration = 1f;
        public float towerSpawningDelay = .25f;
        [Header("Catapult")]
        public float catapultProjectileInflection = 10f;
        [Header("Destroyed Tower")]
        public float toTowerCameraMoveTime = 0.5f;
        public float toTowerCameraWait = 2f;
        public BoatPushSettings pushSettings;
        [Header("Sinking")] 
        public float sinkingDelay;
        [Header("Layers")] 
        public LayerMask blockMask;
        public LayerMask damageableMask;

        // private void OnEnable()
        // {
        //     SetupStaticFields();
        // }

        public void SetupStaticFields()
        {
            CaptainRotationLerp = captainRotationLerp;
            BlockMask = blockMask;
            DamageableMask = damageableMask;
            ArrowSpeed = arrowSpeed;
            SinkingDelay = sinkingDelay;
            TowerSpawningDelay = towerSpawningDelay;
            TowerSpawningDuration = towerSpawningDuration;
            PlayerCameraSetTime = playerCameraSetTime;
            PlayerCameraReturnTime = playerCameraReturnTime;
            BoatPartConnectTime = boatPartConnectTime;
            BoatBreakForce = boatBreakForce;
            UnitRagdollForce = unitRagdollForce;
            BoatPushSettings = pushSettings;
            CatapultProjectileInflection = catapultProjectileInflection;
            ToTowerCameraMoveTime = toTowerCameraMoveTime;
            ToTowerCameraWait = toTowerCameraWait;
            TowerUnloadPointsOffset = towerUnloadPointsOffset;
            PlayerFullIndicatorDuration = playerFullIndicatorDuration;
            PlayerBoatMaxConnectionsCount = playerBoatMaxConnectionsCount;
            BombDamage = bombDamage;
            CollisionDamageMultiplier = collisionDamageMultiplier;
            RadiusChangeTime = radiusChangeTime;
            TowerUnitsRotTime = towerUnitsRotTime;
            CastSkippedFrames = castSkippedFrames;
            // debug staff
            TowersNoAttack = towersNoAttack;
            EnemyNotActivated = enemyNotActivated;
            BoatUnitsNoAttack = boatUnitsNoAttack;
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}