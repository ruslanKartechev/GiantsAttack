namespace GiantsAttack
{
    public interface IStageResultListener
    {
        void OnCompleted(LevelStage stage);
        void OnFailed(LevelStage stage);
        void OnWin();
    }
}