#if UNITY_EDITOR
namespace GameCore
{
    public interface ICheatApplier
    {
        void Freeze();
        void Play();
        void LevelWin();
        void LevelFail();
        void Reload();
        void AddMoney(float amount);
        void RemoveMoney(float amount);
        void KillPlayer();
        void KillEnemies();
        void NextStage();
    }
}
#endif