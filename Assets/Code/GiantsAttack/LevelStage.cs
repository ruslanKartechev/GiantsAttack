using System.Collections.Generic;
using GameCore.Cam;
using GameCore.UI;
using SleepDev;
using UnityEngine;

namespace GiantsAttack
{
    public abstract class LevelStage : MonoExtended
    {
        [SerializeField] protected List<StageListener> _stageListeners;
        protected bool _isStopped;

        public virtual IGameplayMenu UI { get; set; }
        public virtual IHelicopter Player { get; set; }
        public virtual IMonster Enemy { get; set; }
        public virtual PlayerCamera Camera { get; set; }
        public virtual IStageResultListener ResultListener { get; set; }
        public virtual IPlayerMover PlayerMover { get; set; }
        
        public abstract void Activate();
        public abstract void Stop();
        
        protected virtual void DestroyPlayerAndFail()
        {
            CLog.LogRed($"{gameObject.name} Stage failed");
            UnsubFromEnemy();
            Player.Kill();
            ResultListener.OnStageFail(this);
        }

        protected virtual void SubToEnemyKill()
        {
            Enemy.OnDefeated -= OnEnemyKilled;
            Enemy.OnDefeated += OnEnemyKilled;
        }

        protected virtual void UnsubFromEnemy()
        {
            Enemy.OnDefeated -= OnEnemyKilled;
        }
        
        protected virtual void OnEnemyKilled(IMonster enemy)
        {
            UnsubFromEnemy();
            Stop();
            ResultListener.OnMainEnemyDead();
        }

        protected virtual void CallCompleted()
        {
            UnsubFromEnemy();
            ResultListener.OnStageComplete(this);
        }
        
    }
}