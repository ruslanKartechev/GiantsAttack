namespace GiantsAttack
{
    
    [System.Serializable]
    public struct ShooterSettings
    {
        public float fireDelay;
        public float speed;
        public UnityEngine.Vector2 damage;
        
        public ShooterSettings(ShooterSettings other)
        {
            fireDelay = other.fireDelay;
            speed = other.speed;
            damage = other.damage;
        }
    }
}