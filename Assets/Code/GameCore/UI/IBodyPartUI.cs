namespace GameCore.UI
{
    public interface IBodyPartUI
    {
        void SetDamageLevel(int level);
        void Animate();
        void SetNonDamageable();
    }
}