namespace GiantsAttack
{
    public interface IStageResultListener
    {
        void OnStageComplete(LevelStage stage);
        void OnStageFail(LevelStage stage);
        void OnMainEnemyDead();
    }
}