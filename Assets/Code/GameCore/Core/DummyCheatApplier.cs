#if UNITY_EDITOR
using UnityEngine;

namespace GameCore
{
    public class DummyCheatApplier : ICheatApplier
    {
        public void Pause()
        {
            Debug.Log($"[Cheat] Pause");
        }

        public void Freeze()
        {
            Debug.Log($"[Cheat] Freeze");
        }

        public void Play()
        {            
            Debug.Log($"[Cheat] Play");
        }

        public void LevelWin()
        {
            Debug.Log($"[Cheat] Win");
        }

        public void LevelFail()
        {
            Debug.Log($"[Cheat] Fail");
        }

        public void Reload()
        {
            Debug.Log($"[Cheat] Reload");
        }

        public void AddMoney(float amount)
        {
            Debug.Log($"[Cheat] Added money");
        }

        public void RemoveMoney(float amount)
        {
            Debug.Log($"[Cheat] Remove money");
        }

        public void KillPlayer()
        {
            Debug.Log($"[Cheat] Kill player");
        }

        public void KillEnemies()
        {
            Debug.Log($"[Cheat] Kill enemies");
        }

        public void NextStage()
        {
            Debug.Log($"[Cheat] Next stage");
        }
    }
}
#endif