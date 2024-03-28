using GameCore.Cam;
using GameCore.UI;
using SleepDev;

namespace GiantsAttack
{
    public abstract class LevelStage : MonoExtended
    {
        protected bool _isStopped;

        public virtual IGameplayMenu UI { get; set; }
        public virtual IHelicopter Player { get; set; }
        public virtual IMonster Enemy { get; set; }
        public virtual PlayerCamera Camera { get; set; }
        public virtual IStageResultListener ResultListener { get; set; }
        
        public abstract void Activate();
        public abstract void Stop();
        
        protected virtual void DestroyPlayerAndFail()
        {
            CLog.LogRed($"{gameObject.name} Stage failed");
            UnsubFromEnemy();
            Player.Kill();
            ResultListener.OnFailed(this);
        }

        protected virtual void SubToEnemyKill()
        {
            Enemy.OnKilled -= OnEnemyKilled;
            Enemy.OnKilled += OnEnemyKilled;
        }

        protected virtual void UnsubFromEnemy()
        {
            Enemy.OnKilled -= OnEnemyKilled;
        }
        
        protected virtual void OnEnemyKilled(IMonster obj)
        {
            UnsubFromEnemy();
            _isStopped = true;
            Stop();
            ResultListener.OnWin();
        }

        protected virtual void CallCompleted()
        {
            UnsubFromEnemy();
            ResultListener.OnCompleted(this);
        }
        
    }
}