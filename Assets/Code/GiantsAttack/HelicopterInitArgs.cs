namespace GiantsAttack
{
    [System.Serializable]
    public class HelicopterInitArgs
    {
        public AimerSettings aimerSettings;
        public ShooterSettings shooterSettings;
        public IHitCounter hitCounter;
    }
}