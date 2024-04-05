using UnityEngine;

namespace GameCore.Core
{
    [CreateAssetMenu(menuName = "SO/GlobalConfig", fileName = "GlobalConfig", order = 0)]
    public class GlobalConfig : ScriptableObject
    {
        /// Constants
        public const string PlayerTag = "Player";

        /// 



        /// Global Static Config
        public static LayerMask BulletMask;
        /// 

        public LayerMask bulletMask;
        
        public void SetupStaticFields()
        {
            BulletMask = bulletMask;
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}