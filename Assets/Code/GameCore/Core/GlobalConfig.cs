using UnityEngine;

namespace GameCore.Core
{
    [CreateAssetMenu(menuName = "SO/GlobalConfig", fileName = "GlobalConfig", order = 0)]
    public class GlobalConfig : ScriptableObject
    {
        /// Constants
        public const string PlayerTag = "Player";

        /// Global Static Config
        public static LayerMask BulletMask;
        public static float SlowMoBulletSpeedMult = 1f;
        public static float SlowMoFireDelayDiv = 1f;
        public static float DamageMultiplier =1f;
        public static float HeadshotDamageMultiplier = 2f;

        /// 

        public LayerMask bulletMask;
        public float damageMultiplier;
        public float slowMoBulletSpeedMult = 1f;
        public float slowMoFireDelayDiv = 1f;
        public float headshotDamageMultiplier = 2f;
        
        public void SetupStaticFields()
        {
            BulletMask = bulletMask;
            DamageMultiplier = damageMultiplier;
            SlowMoBulletSpeedMult = slowMoBulletSpeedMult;
            SlowMoFireDelayDiv = slowMoFireDelayDiv;
            HeadshotDamageMultiplier = headshotDamageMultiplier;
        }
    }
    

    
    
    
}