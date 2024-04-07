namespace GiantsAttack
{
    public interface IRunaway
    {
        void Init();
        void BeginMoving();
        void Stop();
        void Kill();
        SplineMover Mover { get; }
    }
}