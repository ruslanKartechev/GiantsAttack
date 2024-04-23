namespace GiantsAttack
{
    public interface ISpecAttackTarget
    {
        void OnStageBegan();
        void OnAttackBegan();
        void OnAttack();
        void OnCompleted();
    }
}